using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
    private PackedScene optionMenuScene;
    private PackedScene comfirmMenuScene;

	private Button resumeButton;
	private Button optionsButton;
	private Button backToMainButton;
	private AnimationPlayer animationPlayer;
	private PanelContainer panel;
    private AudioStreamPlayer audioStreamPlayer;

    private bool is_closing = false; //标识关闭动画运行状态，避免多次调用
	public override void _Ready()
	{
        optionMenuScene=ResourceLoader.Load<PackedScene>("uid://cxt2kwk3d1ldj");
        comfirmMenuScene = ResourceLoader.Load<PackedScene>("uid://d1j10w4xi35dh");

        resumeButton = GetNode<Button>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/ResumeButton");
		optionsButton = GetNode<Button>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/OptionsButton");
		backToMainButton = GetNode<Button>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/BackToMainButton");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		panel = GetNode<PanelContainer>("MarginContainer/PanelContainer");
        audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

        resumeButton.Pressed += OnResumeButtonPressed;
        optionsButton.Pressed += OnOptionsButtonPressed;
        backToMainButton.Pressed += OnBackToMainButtonPressed;

		GetTree().Paused = true;
        audioStreamPlayer.Play();
        animationPlayer.Play("In");

        panel.PivotOffset = panel.Size / 2;
		var tween = CreateTween();
		tween.TweenProperty(panel, "scale", Vector2.Zero, 0);
		tween.TweenProperty(panel, "scale", Vector2.One, 0.4)
			.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
    }

    //用于监听未被处理的输入，esc关闭暂停菜单
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Pause"))
        {
            Close();
            GetTree().Root.SetInputAsHandled(); //需要调用此方法表示已处理完毕，跳出该循环
        }
    }

	private async void Close()
	{
        if (is_closing) return;
        is_closing= true;

        audioStreamPlayer.Play();
        animationPlayer.Play("Out");

        var tween = CreateTween();
        tween.TweenProperty(panel, "scale", Vector2.One, 0);
        tween.TweenProperty(panel, "scale", Vector2.Zero, 0.4)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Back);

        await ToSignal(tween, Tween.SignalName.Finished);

        GetTree().Paused = false;
        QueueFree();
    }

    private void OnResumeButtonPressed()
	{
        Close();
    }
    private async void OnOptionsButtonPressed()
    {
        //引入转场动画
        ScreenTransition.Instance.Transition();
        await ToSignal(ScreenTransition.Instance, ScreenTransition.SignalName.HalfScreenTransition);

        var optionMenu = optionMenuScene.Instantiate() as OptionsMenu;
        AddChild(optionMenu);
        optionMenu.RemoveChild(optionMenu.GetNode<CanvasLayer>("Vignette"));
        optionMenu.BackButtonPressed += () => OnBackButtonPressed(optionMenu);
    }
    private void OnBackToMainButtonPressed()
    {
        var comfirmMenu = comfirmMenuScene.Instantiate() as ComfirmMenu;
        AddChild(comfirmMenu);
        comfirmMenu.GetNode<Label>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/ComfirmLabel").Text = "确认放弃本局游戏并返回标题？";
        comfirmMenu.Comfirmed += OnComfirmed;
    }
    private void OnBackButtonPressed(OptionsMenu optionMenu)
    {
        optionMenu.QueueFree();
    }
    //接收到确认信号，返回主菜单
    private async void OnComfirmed()
    {
        //引入转场动画
        ScreenTransition.Instance.Transition();
        MetaProgression.Instance.SaveToSavefile();
        await ToSignal(ScreenTransition.Instance, ScreenTransition.SignalName.HalfScreenTransition);

        GetTree().Paused = false;
        GetTree().ChangeSceneToFile("uid://dubuxra5s3tli");
    }
}
