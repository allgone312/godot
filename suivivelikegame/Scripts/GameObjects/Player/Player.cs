using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	private static Player _instance;
    public static Player Instance { get { return _instance; } }
    public override void _EnterTree()
    {
        if (_instance != null)
        {
            // 如果已存在实例，销毁自身
            QueueFree();
            return;
        }
        _instance = this;
    }

    // 释放时清理静态引用
    public override void _ExitTree()
    {
        if (_instance == this)
        {
            _instance = null;
        }
        GameEvents.Instance.AddedUpgrade -= OnAddedUpgrade; //对Autoload的文件记得释放连接的信号
    }

    [Export] public HealthComponent healthComponent;

    private VelocityComponent velocityComponent;
	private Vector2 _direction = Vector2.Zero;
    private Area2D hurtArea2D;
    private Timer invencibleTimer;
    private ProgressBar HealthBar;
    private AnimatedSprite2D animatedSprite;
    private RandomAudioComponent audioComponent;
    private int collision_count = 0; //同时与玩家碰撞的敌人数量
    private int look_dir = 0;
    private float base_speed = 0.0f; 

    public override void _Ready()
    {
        if (healthComponent == null) { GD.Print("Player.cs未给healthComponent赋值"); return; }
        velocityComponent = GetNode<VelocityComponent>("VelocityComponent");
        base_speed = velocityComponent.max_speed;
        animatedSprite = GetNode<AnimatedSprite2D>("PlayerAnimation");
        invencibleTimer = GetNode<Timer>("InvencibleTimer");
        invencibleTimer.Timeout += OnInvencibleTimeOut;
        hurtArea2D = GetNode<Area2D>("HurtArea2D");
        hurtArea2D.BodyEntered += OnBodyEntered; //用于检测与地方单位的碰撞
        hurtArea2D.BodyExited += OnBodyExited;
        hurtArea2D.AreaEntered += OnAreaEntered; //用于检测与敌方射弹的碰撞
        hurtArea2D.AreaExited += OnAreaExited;
        HealthBar = GetNode<ProgressBar>("HealthBar");
        audioComponent = GetNode<RandomAudioComponent>("RandomAudioComponent");
        Update_HealthBar();
        healthComponent.HealthReduced += OnHealthReduced;
        GameEvents.Instance.AddedUpgrade += OnAddedUpgrade; //当添加一种全新武器时      
    }

    public override void _PhysicsProcess(double delta)
	{
		_direction = GetMovementVector().Normalized();

        velocityComponent.AccelerateInDirection(_direction);
        velocityComponent.Move(this);
        SetAnimation(_direction);
    }

	private Vector2 GetMovementVector()
	{
		Vector2 movementVector = Vector2.Zero;
		movementVector.X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
		movementVector.Y= Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up");
        //设置移动动画朝向
        if (movementVector.X < 0) 
        { 
            animatedSprite.FlipH = true; 
        }
        else if(movementVector.X > 0)
        {
            animatedSprite.FlipH = false;
        }
		return movementVector;
    }

    private void CheckDamage()
    {
        if(collision_count==0 || !invencibleTimer.IsStopped()) { return; }
        healthComponent.Damage(1);
        invencibleTimer.Start();    
    }
    private void Update_HealthBar()
    {
        HealthBar.Value = healthComponent.GetHealthPersent();
    }
    private void SetAnimation(Vector2 dir)
    {
        if (dir==Vector2.Zero)
        {
            animatedSprite.Play("Idle");
        }
        else
        {
            animatedSprite.Play("Walk");
        }
    }

    private void IncreseMoveSpeed(int count)
    {
        velocityComponent.max_speed = base_speed * (1 + 0.1f * count);
    }

    private void OnBodyEntered(Node2D body)
    {
        collision_count++;
        CheckDamage();
    }
    private void OnBodyExited(Node2D body)
    { 
        collision_count=Math.Max(collision_count-1, 0);
    }
    private void OnAreaEntered(Area2D area)
    {
        collision_count++;
        CheckDamage();
    }
    private void OnAreaExited(Area2D area)
    {
        collision_count = Math.Max(collision_count - 1, 0);
    }
    private void OnInvencibleTimeOut()
    {
        CheckDamage(); 
    }
    private void OnHealthReduced()
    {
        Update_HealthBar();
        GameEvents.Instance.Emit_PlayerHurt();
        audioComponent.RandomPlay(); //播放受伤声音
    }
    //若选择的是全新能力卡片，那么添加对应的controller到节点下
    private void OnAddedUpgrade(string id, Dictionary<string, Ability_Upgrades> current_upgrade, Dictionary<string, int> current_upgrade_count)
    {
        if (current_upgrade[id] is Ability_Itself) 
        {
            var ability_itself = current_upgrade[id] as Ability_Itself;
            var ability_controller = ability_itself.abilityController_scene.Instantiate();
            GetNode<Node>("Abilities").AddChild(ability_controller);
        }
        else
        {
            switch (id)
            {
                case "move_speed":
                    IncreseMoveSpeed(current_upgrade_count[id]);
                    break;
                default: break;
            }
        }
    }
}
