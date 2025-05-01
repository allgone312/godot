using Godot;
using System;

public partial class GameTimeUi : CanvasLayer
{
	[Export] GameTimeManager gameTimeManager;

	private Label label;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(gameTimeManager == null) { return; }
		label = GetNode<Label>("MarginContainer/Label");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		label.Text=Time_Format(gameTimeManager.GetTime());
	}

	private string Time_Format(double time)
	{
		var minutes = Math.Floor(time / 60);
		var seconds = Math.Floor(time - minutes * 60);
		return minutes + ":" + seconds;
	}
}
