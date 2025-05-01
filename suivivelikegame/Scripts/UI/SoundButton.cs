using Godot;
using System;

public partial class SoundButton : Button
{
	[Export] public UiAudioPlayerComponent uiAudioComponent;

    public override void _Ready()
    {
        this.Pressed += OnButtonPressed;
        this.MouseEntered += OnMouseEntered;
    }

    private void OnButtonPressed()
    {
        uiAudioComponent.RandomPlay();
    }
    private void OnMouseEntered()
    {
        if(Disabled) return;
        uiAudioComponent.RandomPlay();
    }
}
