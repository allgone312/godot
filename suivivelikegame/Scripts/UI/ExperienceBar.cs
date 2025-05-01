using Godot;
using System;

public partial class ExperienceBar : CanvasLayer
{
	[Export] public  ExperienceManager experienceManager;

	private ProgressBar progressBar;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		experienceManager.ExperienceBar_Updated += OnUpdateBar;
		progressBar = GetNode<ProgressBar>("MarginContainer/ProgressBar");
		progressBar.Value = 0;
	}

	private void OnUpdateBar(float currentExp,float targetExp) 
	{
		if (targetExp <= 0) targetExp = 5.0f;
		progressBar.Value = currentExp / targetExp;
	}
}
