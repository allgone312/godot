using Godot;
using System;
using System.Collections.Generic;

public partial class GameEvents : Node
{
    //事件中心。对于AutoLoad节点创建一个全局单例
    private static GameEvents _instance;
    public static GameEvents Instance => _instance;
    public override void _EnterTree()
    {
        if (_instance != null)
        {
            QueueFree();
            return;
        }
        _instance = this;
    }
    public override void _ExitTree()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    //自定义信号，当收集到经验瓶时
    [Signal]
    public delegate void ExperienceVialCollectedEventHandler(float number); 
    //发射信号
    public void Emit_ExperienceVialCollected(float number)
    {
        EmitSignal(SignalName.ExperienceVialCollected, number);
    }

    //自定义信号，当选择了升级项时
    //不加[signal]标识了，特么godot对C#支持还是差。加上都不能用来传字典字段。用原生C#委托和事件。
    public delegate void UpgradeEventHandler(string id,Dictionary<string, Ability_Upgrades> current_upgrade, Dictionary<string, int> current_upgrade_count);
    public event UpgradeEventHandler AddedUpgrade;
    //发射信号
    public void Emit_AddedUpgrade(string id,Dictionary<string, Ability_Upgrades> current_upgrade, Dictionary<string, int> current_upgrade_count)
    {
        AddedUpgrade(id,current_upgrade, current_upgrade_count);
    }

    //当玩家受到攻击时
    [Signal]
    public delegate void PlayerHurtEventHandler();
    //发射信号
    public void Emit_PlayerHurt()
    {
        EmitSignal(SignalName.PlayerHurt);
    }
}
