using Godot;
using System;

public partial class BasicBullet : Node2D
{
	private Sprite2D sprite;
    private GpuParticles2D particles;
	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("BulletSprite2D");
        particles = GetNode<GpuParticles2D>("GPUParticles2D");
        particles.Emitting = false;
	}

	public void ShootBullet(Vector2 direction)
	{
        var distance = GlobalPosition + direction * 300;
        var tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.Zero * 1.0f, 0);
        tween.TweenProperty(this, "scale", Vector2.One * 1.0f, 0.4f)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);

        tween.TweenProperty(particles, "emitting", true, 0);
        tween.SetParallel();       
        tween.TweenProperty(sprite, "rotation", Mathf.RadToDeg(Mathf.Pi * 8), 4.0f);
        tween.TweenProperty(this, "global_position", distance, 4.0f);
        tween.Chain();
        tween.TweenProperty(particles, "emitting", false, 0);
        tween.TweenProperty(this, "scale", Vector2.Zero * 1.0f, 0.4f)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
        tween.Chain();
        tween.TweenCallback(Callable.From(QueueFree));
    }
}
