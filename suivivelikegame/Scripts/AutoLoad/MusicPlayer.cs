using Godot;
using System;

public partial class MusicPlayer : AudioStreamPlayer
{
    private Timer timer;
    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        Finished += OnFinished;
        timer.Timeout += OnTimeOut;
    }

    private void OnFinished()
    {
        timer.Start();
    }
    private void OnTimeOut()
    {
        Play();
    }
}
