using Godot;
using System;

public partial class GameTimeManager : Node
{
    [Export] public PackedScene game_end_scene;
    [Signal] public delegate void DifficultyIncreasedEventHandler(float difficulty);

    private const float DIFFICULTY_INCREASE_INTERVAL = 3.0f; //随时间难度增长的间隔
    private float current_difficulty = 0.0f;

    private Timer _timer;

    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _timer.Timeout += OnTimeOut;
    }
    public override void _Process(double delta)
    {
        //随时间增加游戏难度
        var next_difficulty_increase_time = _timer.WaitTime - DIFFICULTY_INCREASE_INTERVAL * (current_difficulty + 1);
        if(_timer.TimeLeft < next_difficulty_increase_time)
        {
            current_difficulty++;
            EmitSignal(SignalName.DifficultyIncreased, current_difficulty);
        }
    }

    public double GetTime()
    {
        return _timer.WaitTime - _timer.TimeLeft;
    }

    private void OnTimeOut()
    {
        var game_end_screen= game_end_scene.Instantiate() as GameEndScreen;
        AddChild(game_end_screen);
        game_end_screen.Set_Victory();

        //一局结束结算当局获得的点数
        MetaProgression.Instance.SaveToSavefile();
    }
}
