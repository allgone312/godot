using Godot;
using System;

public partial class ExperienceManager : Node
{
    [Signal] public delegate void ExperienceBar_UpdatedEventHandler(float currentExp,float targetExp);
    [Signal] public delegate void LevelUpEventHandler(float newLevel);

    private const float EXPERIENCE_GROWTH = 5.0f;
    private float currentExp = 0.0f;
    private float targetExp = 1.0f;
    private float currentLevel = 1.0f;

    public override void _Ready()
    {
        //连接自定义信号
        GameEvents.Instance.ExperienceVialCollected += OnIncrementExperience;
    }

    public void IncrementExperience(float number)
    {
        //发送增长经验条信号
        currentExp = Mathf.Min(currentExp + number, targetExp);
        EmitSignal(SignalName.ExperienceBar_Updated, currentExp, targetExp);
        //收集满经验升级
        if (currentExp == targetExp)
        {
            currentExp = 0;
            targetExp += EXPERIENCE_GROWTH;
            currentLevel += 1;
            EmitSignal(SignalName.ExperienceBar_Updated, currentExp, targetExp);
            EmitSignal(SignalName.LevelUp, currentLevel);
        }
    }
    //连接信号的函数，分开写。以便里边的功能能让其他地方使用
    private void OnIncrementExperience(float number)
    {
        IncrementExperience(number);
    }
    //对autoload文件中信号需要手动断开连接
    public override void _ExitTree()
    {
        GameEvents.Instance.ExperienceVialCollected -= OnIncrementExperience;
    }
}
