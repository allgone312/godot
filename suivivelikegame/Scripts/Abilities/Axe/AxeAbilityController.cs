using Godot;
using System;
using System.Collections.Generic;

public partial class AxeAbilityController : Node
{
    [Export] public float axeDamage = 10.0f;
    [Export] public PackedScene axeAbilityScene;

    private Timer timer;
    private Node foreground_layer;
    public override void _Ready()
    {
        foreground_layer = GetTree().GetFirstNodeInGroup("Foreground_Layer");
        timer = GetNode<Timer>("Timer");
        timer.Timeout += OnTimeOut;
        GameEvents.Instance.AddedUpgrade += OnAddedUpgrade;
    }

    private void OnTimeOut()
    {
        var axeAbility= axeAbilityScene.Instantiate() as AxeAbility;
        //设置斧子初始位置
        axeAbility.GlobalPosition = Player.Instance.GlobalPosition;
        //设置伤害
        axeAbility.GetNode<HitBoxComponent>("HitBoxComponent").damage = axeDamage;
        foreground_layer.AddChild(axeAbility);        
    }
    private void OnAddedUpgrade(string id, Dictionary<string, Ability_Upgrades> current_upgrade, Dictionary<string, int> current_upgrade_count)
    {
        switch (id)
        {            
            case "axe_damage":
                ApplySwordDamage(current_upgrade_count["axe_damage"]); break;
            default: break;
        }
    }
    //对autoload文件中信号需要手动断开连接
    public override void _ExitTree()
    {
        GameEvents.Instance.AddedUpgrade -= OnAddedUpgrade;
    }

    //提高伤害
    private void ApplySwordDamage(int count)
    {
        float addtional_persent = 1 + count * 0.10f;
        axeDamage *= addtional_persent;
    }
}
