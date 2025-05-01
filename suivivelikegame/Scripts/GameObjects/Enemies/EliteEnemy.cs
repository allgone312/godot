using Godot;
using System;
using static Godot.TextServer;

public partial class EliteEnemy : CharacterBody2D
{
    [Export] public float enemy_damage = 1.0f;

    private const float ATTACK_RANGE = 80.0f;

    private Vector2 _direction = Vector2.Zero;
    private AnimationTree animationTree;
    private VelocityComponent velocityComponent;
    private CollisionShape2D bodyCollision;
    private CollisionShape2D chargeCollision;
    private Area2D chargeCollisionArea;
    private Timer attactTimer;
    private Tween move_tween;
    private Tween callback_tween;
    
    private bool is_charging; //是否在冲锋
    private bool is_can_change;
    public override void _Ready()
    {
        is_charging = false;
        is_can_change = true;
        
        animationTree = GetNode<AnimationTree>("AnimationTree");
        velocityComponent = GetNode<VelocityComponent>("VelocityComponent");
        bodyCollision = GetNode<CollisionShape2D>("CollisionShape2D");
        chargeCollision = GetNode<CollisionShape2D>("ChargeCollision/CollisionShape2D");
        chargeCollisionArea = GetNode<Area2D>("ChargeCollision");
        attactTimer = GetNode<Timer>("AttackTimer");

        chargeCollisionArea.BodyEntered += OnEnteredWall;
        attactTimer.Timeout += OnTimeOut;

        bodyCollision.Disabled= false;
        chargeCollision.Disabled = true;

        animationTree.Active = true;
    }
    public override void _PhysicsProcess(double delta)
    {       
        if (GlobalPosition.DistanceSquaredTo(Player.Instance.GlobalPosition) > Math.Pow(ATTACK_RANGE, 2))
        {
            velocityComponent.AccelerateToPlayer();
            velocityComponent.Move(this);
        }
        else
        {
            Velocity = Vector2.Zero;
            MoveAndSlide();
        }
        _direction = velocityComponent.GetDirectionToPlayer();
        SetAnimation(_direction);
    }

    //更新动画状态机
    private void SetAnimation(Vector2 dir)
    {
        if (Velocity == Vector2.Zero)
        {
            animationTree.Set("parameters/conditions/attack", false);
            animationTree.Set("parameters/conditions/idle", true);
            animationTree.Set("parameters/conditions/is_walking", false);
        }
        else
        {
            animationTree.Set("parameters/conditions/attack", false);
            animationTree.Set("parameters/conditions/idle", false);
            animationTree.Set("parameters/conditions/is_walking", true);
        }
        if ( (GlobalPosition.DistanceSquaredTo(Player.Instance.GlobalPosition) < Math.Pow(ATTACK_RANGE, 2) && is_can_change) || is_charging )
        {
            animationTree.Set("parameters/conditions/attack", true);
            animationTree.Set("parameters/conditions/idle", false);
            animationTree.Set("parameters/conditions/is_walking", false);
            ChargeAttack(dir);
        }
        if (dir != Vector2.Zero)
        {
            var _dir = dir.X > 0 ? 1.0 : -1.0;
            if (!is_charging)
            {
                animationTree.Set("parameters/Attack/blend_position", _dir);
            }           
            animationTree.Set("parameters/Idle/blend_position", _dir);
            animationTree.Set("parameters/Walk/blend_position", _dir);
        }
    }

    private async void ChargeAttack(Vector2 direction)
    {
        if (!is_can_change) { return; }
        is_can_change = false;
        is_charging = true;

        bodyCollision.Disabled = true;  //开始冲锋时取消敌人间的相互碰撞
        chargeCollision.Disabled = false;

        await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);

        var distance = GlobalPosition + direction * 150;
        move_tween = CreateTween();
        move_tween.TweenProperty(this, "global_position", distance, 1.0f);

        callback_tween = CreateTween();
        callback_tween.TweenCallback(Callable.From(ChargeComplete)).SetDelay(1.4f);
    }
    private void ChargeComplete()
    {
        is_charging = false;

        bodyCollision.Disabled = false;  
        chargeCollision.Disabled = true;
        attactTimer.Start();
    }

    private void OnTimeOut()
    {
        is_can_change = true;
    }

    private void OnEnteredWall(Node2D tileMap)
    {
        if (move_tween != null)
        {
            float elapsed = (float)move_tween.GetTotalElapsedTime();
            float remainingTime = 1.0f - elapsed;

            move_tween.Stop();
            move_tween.Kill();
        }
    }
}
