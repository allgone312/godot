using Godot;
using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Linq.Expressions;

public partial class SwordAbilibyController : Node
{
    [Export] public float swordDamage = 5.0f;
    [Export] public PackedScene swordScene;

    private const float MAX_RANGE = 100.0f;
    private const float MAX_PERSENT = 0.5f;
    private float default_wait_time;
    private Timer timer;
    private Node2D nearestEnemy=null;
    private Node foreground_layer;

    public override void _Ready()
    {
        foreground_layer = GetTree().GetFirstNodeInGroup("Foreground_Layer");
        timer = GetNode<Timer>("Timer");
        default_wait_time = (float)timer.WaitTime;
        timer.Timeout += OnTimerOut;
        GameEvents.Instance.AddedUpgrade += OnAddedUpgrade;
    }

    private void OnTimerOut()
    {       
        GetNearestEnemy(ref nearestEnemy);
        if (nearestEnemy != null)
        {
            var swordAbility = swordScene.Instantiate() as Node2D;
            //将最近敌人的位置赋值
            swordAbility.GlobalPosition = nearestEnemy.GlobalPosition;
            //设置剑能力的伤害
            swordAbility.GetNode<HitBoxComponent>("HitBoxComponent").damage= swordDamage;
            //让剑位置向玩家方向偏移几个像素
            var offsetDir=Player.Instance.GlobalPosition- nearestEnemy.GlobalPosition;
            swordAbility.GlobalPosition += offsetDir.Normalized()*20;
            //剑朝向敌人
            var enemyDir=nearestEnemy.GlobalPosition-swordAbility.GlobalPosition;
            swordAbility.Rotation = enemyDir.Angle();
            foreground_layer.AddChild(swordAbility);
        }
    }
    //寻找最近的目标
    private void GetNearestEnemy(ref Node2D nearestEnemy)
    {
        var enemies = GetTree().GetNodesInGroup("enemy").OfType<Node2D>().ToArray();
        if (enemies.Length > 0)
        {
            enemies = enemies.Where(e=>e.GlobalPosition.DistanceSquaredTo(Player.Instance.GlobalPosition)<Math.Pow(MAX_RANGE,2)).ToArray();
            nearestEnemy = enemies.MinBy(n=>n.GlobalPosition.DistanceSquaredTo(Player.Instance.GlobalPosition));
        }
        else
        {
            nearestEnemy = null;
        }
    }

    //接收到事件中心发出的技能升级信号
    private void OnAddedUpgrade(string id, Dictionary<string, Ability_Upgrades> current_upgrade, Dictionary<string, int> current_upgrade_count)
    {
        switch (id)
        {
            case "sword_rate":
                ApplySwordRate(current_upgrade_count["sword_rate"]); break;
            case "sword_damage":
                ApplySwordDamage(current_upgrade_count["sword_damage"]); break;
            default: break;
        }
    }
    //对autoload文件中信号需要手动断开连接
    public override void _ExitTree()
    {
        GameEvents.Instance.AddedUpgrade -= OnAddedUpgrade;
    }

    //提高攻速
    private void ApplySwordRate(int count)
    {
        var persent = count * 0.1; //攻速提高百分比
        persent = Math.Min(persent, MAX_PERSENT); //攻速最高提高到50%
        timer.WaitTime = default_wait_time * (1 - persent); //攻击间隔时间减少
        timer.Start(); //重设了wait_time则timer组件也要重新开始计时
    }
    //提高伤害
    private void ApplySwordDamage(int count)
    {
        float addtional_persent = 1 + count * 0.15f;
        swordDamage *= addtional_persent;
    }
}
