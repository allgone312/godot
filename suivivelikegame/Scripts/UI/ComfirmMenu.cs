using Godot;
using System;

public partial class ComfirmMenu : CanvasLayer
{
	[Signal] public delegate void ComfirmedEventHandler();

	private Button comfirmButton;
	private Button cancelButton;
	private AnimationPlayer animationPlayer;
	private PanelContainer panel;

    public override void _Ready()
	{
        comfirmButton = GetNode<Button>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/ComfirmButton");
		cancelButton = GetNode<Button>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/CancelButton");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		panel = GetNode<PanelContainer>("MarginContainer/PanelContainer");		
		comfirmButton.Pressed += OnComfirmPressed;
		cancelButton.Pressed += OnCancelPressed;

        animationPlayer.Play("In");

        panel.PivotOffset = panel.Size / 2;
        var tween = CreateTween();
        tween.TweenProperty(panel, "scale", Vector2.Zero, 0);
        tween.TweenProperty(panel, "scale", Vector2.One, 0.4)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
    }

	private void OnComfirmPressed()
	{
		EmitSignal(SignalName.Comfirmed);
	}
	private async void OnCancelPressed()
	{
        animationPlayer.Play("Out");

        var tween = CreateTween();
        tween.TweenProperty(panel, "scale", Vector2.One, 0);
        tween.TweenProperty(panel, "scale", Vector2.Zero, 0.4)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);

        await ToSignal(tween, Tween.SignalName.Finished);
        QueueFree();
	}
}
