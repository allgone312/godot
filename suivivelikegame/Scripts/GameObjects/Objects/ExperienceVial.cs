using Godot;
using System;

public partial class ExperienceVial : Node2D
{
    [Export] public float exp = 1.0f;
    [Export] public RandomAudioComponent randomAudioComponent;

    private const float MOVE_SPEED = 400.0f;
    private Area2D area;
    private CollisionShape2D collisionShape;
    private Sprite2D sprite;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        collisionShape = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
        sprite = GetNode<Sprite2D>("Sprite2D");
        area = GetNode<Area2D>("Area2D");
        area.AreaEntered += OnAreaEntered;
    }

    

    private void CollectTween(float persent,Vector2 start_postion)
    {
        if (Player.Instance == null) { return; }
        persent = persent < 0 ? Mathf.Max(persent, -0.003f) : persent; //对反冲距离设置一个最小值，避免反冲出屏幕
        GlobalPosition = start_postion.Lerp(Player.Instance.GlobalPosition, persent);
        var direction = Player.Instance.GlobalPosition - start_postion;

        var target_rotation = direction.Angle() + Mathf.DegToRad(90);
        Rotation = Mathf.LerpAngle(Rotation, target_rotation, 1 - (float)Math.Exp(-2 * GetProcessDeltaTime()));
    }
    private void Collected()
    {
        //当拾取一个经验瓶，发出信号
        GameEvents.Instance.Emit_ExperienceVialCollected(exp);
        QueueFree();
    }
    private void DisableCollision()
    {
        collisionShape.Disabled = true;
    }
    private void OnAreaEntered(Area2D area2D)
    {
        CallDeferred(nameof(DisableCollision)); //捡到小瓶发生一次碰撞，小瓶追上玩家又会发生一次碰撞，因此在第一次碰撞后禁用碰撞
        //设置经验瓶飞向玩家的动画
        var tween = CreateTween();
        tween.SetParallel(); //设置以下tween动画为并行动画
        tween.TweenMethod(Callable.From((float persent) => CollectTween(persent, GlobalPosition)), 0.0f, 1.0f, 0.5f)
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Back); //设定插值曲线，同时将当前GlobalPosition作为参数绑定到方法中。网站easings.net可查看各对应数学曲线。C#用Back曲线似乎有bug? 会直接反向飞出屏幕

        tween.TweenProperty(sprite, "scale", Vector2.Zero, 0.15).SetDelay(0.35); //并行动画。动画时长+delay时长要与上边动画相等

        tween.Chain(); //表示不与上边并行动画进行，会在上边两动画完成后再执行下边动画
        tween.TweenCallback(Callable.From(Collected)); //动画完成后

        randomAudioComponent.RandomPlay();
    }
}
