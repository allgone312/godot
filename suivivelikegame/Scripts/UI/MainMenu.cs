using Godot;
using System;

public partial class MainMenu : CanvasLayer
{
    private PackedScene metaMenuScene;
    private PackedScene optionMenuScene;	

	private MarginContainer marginContainer;
	private Button startButton;
	private Button upgradeButton;
	private Button optionsButton;
	private Button quitButton;
	public override void _Ready()
	{
		metaMenuScene = ResourceLoader.Load<PackedScene>("uid://dkoa4qen2v4cm");
		optionMenuScene = ResourceLoader.Load<PackedScene>("uid://cxt2kwk3d1ldj");

		marginContainer = GetNode<MarginContainer>("MarginContainer");
        startButton = GetNode<Button>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/StartButton");
		upgradeButton = GetNode<Button>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/UpgradeButton");
        optionsButton = GetNode<Button>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/OptionsButton");
        quitButton = GetNode<Button>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/QuitButton");

		startButton.Pressed += OnStartPressed;
		upgradeButton.Pressed += OnUpgradePressed;
        optionsButton.Pressed += OnOptionsPressed;
        quitButton.Pressed += OnQuitPressed;
    }

	private async void OnStartPressed()
	{
		//引入转场动画
		ScreenTransition.Instance.Transition();
		await ToSignal(ScreenTransition.Instance, ScreenTransition.SignalName.HalfScreenTransition);

		GetTree().ChangeSceneToFile("uid://c4r4jxy0v85lm");
	}
	private async void OnUpgradePressed()
	{
        //引入转场动画
        ScreenTransition.Instance.Transition();
        await ToSignal(ScreenTransition.Instance, ScreenTransition.SignalName.HalfScreenTransition);

		marginContainer.Visible = false;
        var metaMenu = metaMenuScene.Instantiate() as MetaMenu;
        AddChild(metaMenu);
        metaMenu.BackButtonPressed += () => OnUpgradeBackPressed(metaMenu);
    }

    private async void OnOptionsPressed()
	{
        //引入转场动画
        ScreenTransition.Instance.Transition();
        await ToSignal(ScreenTransition.Instance, ScreenTransition.SignalName.HalfScreenTransition);

        var optionMenu = optionMenuScene.Instantiate() as OptionsMenu;
		AddChild(optionMenu);
		optionMenu.BackButtonPressed += ()=> OnBackButtonPressed(optionMenu);
    }
	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
	private void OnBackButtonPressed(OptionsMenu optionMenu)
	{
		optionMenu.QueueFree();
	}
	private void OnUpgradeBackPressed(MetaMenu metaMenu)
	{
		metaMenu.QueueFree();
		marginContainer.Visible = true;
	}
}
