using Godot;
using System;

public partial class ScreenTransition : CanvasLayer
{
    private static ScreenTransition _instance;
    public static ScreenTransition Instance { get { return _instance; } }
    public override void _EnterTree()
    {
        if (_instance != null)
        {
            // 如果已存在实例，销毁自身
            QueueFree();
            return;
        }
        _instance = this;
    }

    // 释放时清理静态引用
    public override void _ExitTree()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }


    [Signal] public delegate void HalfScreenTransitionEventHandler();

	private AnimationPlayer animationPlayer;
	private bool skip_emit=false;
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public async void Transition()
	{
		animationPlayer.Play("trans");
		await ToSignal(this, SignalName.HalfScreenTransition); //屏幕全部遮住之后，发送信号进行后台场景切换。等待加载完成后，再重新亮出屏幕
		skip_emit = true;
		animationPlayer.PlayBackwards("trans");
	}
	public void Emit_HalfScreenTransition()
	{
		//避免两次调用发送信号
		if (skip_emit)
		{
			skip_emit= false;
			return;
		}
		EmitSignal(SignalName.HalfScreenTransition);
	}
}
