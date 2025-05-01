using Godot;
using System;

public partial class Vignette : CanvasLayer
{
	private AnimationPlayer animePlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animePlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        GameEvents.Instance.PlayerHurt += OnPlayerHurt;
	}
    public override void _ExitTree()
    {
        GameEvents.Instance.PlayerHurt -= OnPlayerHurt; //autoLoad文件中信号要记得手动断开
    }

    private void OnPlayerHurt()
	{
		animePlayer.Play("hit");
	}
}
