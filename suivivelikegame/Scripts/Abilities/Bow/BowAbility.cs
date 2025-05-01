using Godot;
using System;

public partial class BowAbility : Node2D
{
    private Area2D checkCollide;

    private Tween tween;
    public override void _Ready()
    {
        checkCollide = GetNode<Area2D>("CheckCollideArea");
        checkCollide.BodyEntered += OnBodyEntered;
    }
    public void ShootBullet(Vector2 direction)
    {
        var distance = GlobalPosition + direction * 150;
        tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.Zero * 1.0f, 0);
        tween.Parallel().TweenProperty(this, "scale", Vector2.One * 1.0f, 0.4f)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        tween.Parallel().TweenProperty(this, "global_position", distance, 1.0f);
        tween.TweenProperty(this, "scale", Vector2.Zero * 1.0f, 0.4f)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
        tween.TweenCallback(Callable.From(QueueFree));
    }

    private void OnBodyEntered(Node2D body)
    {
        if (tween == null) { return; }
        tween.Stop();
        tween.Kill();
        tween=CreateTween();
        tween.TweenProperty(this, "scale", Vector2.Zero * 1.0f, 0.2f)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
        tween.TweenCallback(Callable.From(QueueFree));
    }
}
