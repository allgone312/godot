using Godot;
using System;

public partial class GameEndScreen : CanvasLayer
{
	private Button restart_button;
	private Button quit_button;
	private Label title_label;
	private Label description_label;
	private PanelContainer panel;
	private AudioStreamPlayer victoryStream;
	private AudioStreamPlayer defeatStream;
    public override void _Ready()
	{
		title_label = GetNode<Label>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/TitleLabel");
		description_label=GetNode<Label>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/DescriptionLabel");
		panel = GetNode<PanelContainer>("MarginContainer/PanelContainer");

		panel.PivotOffset = panel.Size / 2;
        //panel.Scale = Vector2.Zero;  creatTween()会在创建时将scale状态重置到默认，因此多用一个TweenProperty来设置
        Tween tween = CreateTween();
        tween.TweenProperty(panel, "scale", Vector2.Zero, 0);
		tween.TweenProperty(panel, "scale", Vector2.One, 0.3)
			.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);

		GetTree().Paused = true;
		restart_button = GetNode<Button>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/VBoxContainer/RestartButton");
		quit_button=GetNode<Button>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/VBoxContainer/QuitButton");
		restart_button.Pressed += OnRestartPressed;
		quit_button.Pressed += OnQuitPressed;

		victoryStream = GetNode<AudioStreamPlayer>("VictoryStreamPlayer");
		defeatStream = GetNode<AudioStreamPlayer>("DefeatStreamPlayer");

    }

	public void Set_Victory()
	{
		title_label.Text = "Victory";
		description_label.Text = "您胜利了！";
		victoryStream.Play();
	}
	public void Set_Defeat()
	{
		title_label.Text = "Game Over";
		description_label.Text = "再接再厉";
		defeatStream.Play();
	}
	private async void OnRestartPressed()
	{
        //引入转场动画
        ScreenTransition.Instance.Transition();
        await ToSignal(ScreenTransition.Instance, ScreenTransition.SignalName.HalfScreenTransition);

        //要记得将Process-> Mode设置为Always，才能在全局暂停时能够操作按钮
        GetTree().Paused=false;
		GetTree().ChangeSceneToFile("res://Scenes/Main/main.tscn");
	}
	private async void OnQuitPressed()
	{
        //引入转场动画
        ScreenTransition.Instance.Transition();
        await ToSignal(ScreenTransition.Instance, ScreenTransition.SignalName.HalfScreenTransition);

        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("uid://dubuxra5s3tli");
    }
}
