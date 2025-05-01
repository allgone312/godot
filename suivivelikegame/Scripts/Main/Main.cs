using Godot;
using System;

public partial class Main : Node2D
{
	private PackedScene game_end_scene;
    private PackedScene pause_menu_scene;

    public override void _Ready()
    {
        game_end_scene = ResourceLoader.Load<PackedScene>("uid://t1b3fxu21vpu");
        pause_menu_scene = ResourceLoader.Load<PackedScene>("uid://c4bgk75g3s52");
        Player.Instance.healthComponent.Died += OnPlayerDied;
    }

    //用于监听未被处理的输入，如esc开启暂停菜单
    public override void _UnhandledInput(InputEvent @event) 
    {
        if (@event.IsActionPressed("Pause"))
        {
            AddChild(pause_menu_scene.Instantiate());
            GetTree().Root.SetInputAsHandled(); //需要调用此方法表示已处理完毕，跳出该循环
        }
    }

    private void OnPlayerDied()
    {
        var game_end_screen = game_end_scene.Instantiate() as GameEndScreen;
        AddChild(game_end_screen);
        game_end_screen.Set_Defeat();

        //一局结束结算当局获得的点数
        MetaProgression.Instance.SaveToSavefile();
    }
}
