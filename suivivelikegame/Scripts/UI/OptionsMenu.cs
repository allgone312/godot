using Godot;
using System;

public partial class OptionsMenu : CanvasLayer
{
	[Signal] public delegate void BackButtonPressedEventHandler();

	private HSlider masterSlider;
	private HSlider musicSlider;
	private HSlider sfxSlider;
	private Button windowButton;
	private Button backbutton;

    public override void _Ready()
	{
		masterSlider = GetNode<HSlider>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/MasterContainer/MasterSlider");
        musicSlider = GetNode<HSlider>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/MusicContainer/MusicSlider");
		sfxSlider = GetNode<HSlider>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/SFXContainer/SFXSlider");
		windowButton = GetNode<Button>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/WindowContainer/WindowModeButton");
		backbutton = GetNode<Button>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/BackButton");

        UpdateDisplay();

		masterSlider.ValueChanged += (value) => OnSliderChanged((float)value, "Master");
		musicSlider.ValueChanged += (value) => OnSliderChanged((float)value, "Music");
        sfxSlider.ValueChanged += (value) => OnSliderChanged((float)value, "SFX"); //分别绑定不同参数，使方法复用
        windowButton.Pressed += OnWindowButtonPressed;
		backbutton.Pressed += OnBackPressed;
	}

	private void UpdateDisplay()
	{
        if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen)
        {
            windowButton.Text = "窗口化";
        }
        else
        {
            windowButton.Text = "全屏化";
        }
		masterSlider.Value = GetBusVolumePersent("Master");
		musicSlider.Value = GetBusVolumePersent("Music");
		sfxSlider.Value = GetBusVolumePersent("SFX");
    }
	private float GetBusVolumePersent(string bus_name)
	{
		var bus_index=AudioServer.GetBusIndex(bus_name);
		var volume_db=AudioServer.GetBusVolumeDb(bus_index);
		return Mathf.DbToLinear(volume_db);
	}
	private void SetBusVolumePersent(string bus_name,float persent)
	{
        var bus_index = AudioServer.GetBusIndex(bus_name);
		var volume_db = Mathf.LinearToDb(persent);
		AudioServer.SetBusVolumeDb(bus_index, volume_db);
    }


    private void OnSliderChanged(float value,string bus_name)
	{
		SetBusVolumePersent(bus_name, value);
	}
	private void OnWindowButtonPressed()
	{
		var mode= DisplayServer.WindowGetMode();
		if(mode != DisplayServer.WindowMode.Fullscreen)
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }
		else
		{
			DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false); //全屏后godot会自动设置无边框为true，返回窗口化后需要将其重新设置为false
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        }
		UpdateDisplay();
    }
	private async void OnBackPressed()
	{
        //引入转场动画
        ScreenTransition.Instance.Transition();
        await ToSignal(ScreenTransition.Instance, ScreenTransition.SignalName.HalfScreenTransition);

        EmitSignal(SignalName.BackButtonPressed);
	}
}
