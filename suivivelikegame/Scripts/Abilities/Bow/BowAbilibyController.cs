using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BowAbilibyController : Node
{
    [Export] public float bowDamage = 10.0f;
    [Export] public PackedScene bowScene;

    private const float MAX_RANGE = 150.0f;
    private const float MAX_PERSENT = 0.3f;

    private int mutishotLevel = 0;
    private int continuoushotLevel = 0;
    private bool is_piercingshot=false;

    private float default_wait_time;
    private Timer timer;
    private Node2D nearestEnemy = null;
    private Node foreground_layer;

    public override void _Ready()
    {
        foreground_layer = GetTree().GetFirstNodeInGroup("Foreground_Layer");
        timer = GetNode<Timer>("Timer");
        default_wait_time = (float)timer.WaitTime;
        timer.Timeout += OnTimerOut;
        GameEvents.Instance.AddedUpgrade += OnAddedUpgrade;
    }
    //对autoload文件中信号需要手动断开连接
    public override void _ExitTree()
    {
        GameEvents.Instance.AddedUpgrade -= OnAddedUpgrade;
    }
    
    //寻找最近的目标
    private Vector2 GetNearestEnemy()
    {
        var enemies = GetTree().GetNodesInGroup("enemy").OfType<Node2D>().ToArray();
        if (enemies.Length > 0)
        {
            enemies = enemies.Where(e => e.GlobalPosition.DistanceSquaredTo(Player.Instance.GlobalPosition) < Math.Pow(MAX_RANGE, 2)).ToArray();
            nearestEnemy = enemies.MinBy(n => n.GlobalPosition.DistanceSquaredTo(Player.Instance.GlobalPosition));
            if (nearestEnemy != null)
            {
                return nearestEnemy.GlobalPosition;
            }         
        }
        else
        {
            nearestEnemy = null;
        }
        return Vector2.Zero;
    }
    private async void OnTimerOut()
    {
        var nearestEnemyPosition = GetNearestEnemy();
        if (nearestEnemyPosition != Vector2.Zero)
        {          
            //有多重射击升级时
            for (int i = 0; i < continuoushotLevel + 1; i++)
            {
                await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
                for (int j = 0; j < mutishotLevel * 2 + 1; j++)
                {
                    var bowAbility = bowScene.Instantiate() as BowAbility;
                    bowAbility.GlobalPosition = Player.Instance.GlobalPosition;
                    //设置伤害
                    bowAbility.GetNode<HitBoxComponent>("HitBoxComponent").damage = bowDamage;
                    //朝向敌人
                    var enemyDir = nearestEnemyPosition - bowAbility.GlobalPosition;
                    //穿透射击
                    bowAbility.GetNode<CollisionShape2D>("CheckCollideArea/CollisionShape2D").Disabled = is_piercingshot;

                    var rotated_angle =  (j - mutishotLevel) * Mathf.DegToRad(30.0f);
                    var direction = enemyDir.Normalized().Rotated(rotated_angle);
                    bowAbility.Rotation = enemyDir.Angle() + rotated_angle;
                    foreground_layer.AddChild(bowAbility);
                    bowAbility.ShootBullet(direction);
                }                
            }
            
        }
    }

    //接收到事件中心发出的技能升级信号
    private void OnAddedUpgrade(string id, Dictionary<string, Ability_Upgrades> current_upgrade, Dictionary<string, int> current_upgrade_count)
    {
        switch (id)
        {
            case "bow_rate":
                ApplyBowRate(current_upgrade_count["bow_rate"]); break;
            case "bow_damage":
                ApplyBowDamage(current_upgrade_count["bow_damage"]); break;
            case "bow_mutishot":  //多重射击
                mutishotLevel = current_upgrade_count["bow_mutishot"]; break;
            case "bow_continuoushot":  //连续射击
                continuoushotLevel = current_upgrade_count["bow_continuoushot"]; break;
            case "bow_piercingshot":   //穿透射击
                is_piercingshot = true; break;
            default: break;
        }
    }
    

    //提高攻速
    private void ApplyBowRate(int count)
    {
        var persent = count * 0.1; //攻速提高百分比
        persent = Math.Min(persent, MAX_PERSENT); //攻速最高提高到50%
        timer.WaitTime = default_wait_time * (1 - persent); //攻击间隔时间减少
        timer.Start(); //重设了wait_time则timer组件也要重新开始计时
    }
    //提高伤害
    private void ApplyBowDamage(int count)
    {
        float addtional_persent = 1 + count * 0.15f;
        bowDamage *= addtional_persent;
    }
}
