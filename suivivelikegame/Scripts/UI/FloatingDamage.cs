using Godot;
using System;


/*用于显示漂浮的伤害数字*/
public partial class FloatingDamage : Node2D
{
	private Label label;
	public override void _Ready()
	{
		label = GetNode<Label>("Label");

    }

	public void DisplayFloatingDamage(string text)
	{
		label.Text=text;
		Tween tween = CreateTween();
        //tween.Parallel();

        tween.TweenProperty(this, "global_position", GlobalPosition + (Vector2.Up * 16), 0.3)
			.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);		
		
		tween.Chain();
        tween.Parallel();
        tween.TweenProperty(this, "global_position", GlobalPosition + (Vector2.Up * 32), 0.3)
			.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(this, "scale", Vector2.Zero, 0.3)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

		tween.Chain();
        tween.TweenCallback(Callable.From(QueueFree));

		Tween scale_tween= CreateTween();
        scale_tween.TweenProperty(this, "scale", Vector2.One * 1.5f, 0.15)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        scale_tween.TweenProperty(this, "scale", Vector2.One , 0.15)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
    }
}
