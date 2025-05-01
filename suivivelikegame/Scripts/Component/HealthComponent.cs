using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class HealthComponent : Node
{
    [Export] public float health = 10.0f;
    [Signal] public delegate void DiedEventHandler();
    [Signal] public delegate void HealthReducedEventHandler();

    public float current_health = 0;

    public override void _Ready()
    {
        current_health = health;
    }
    public void Damage(float damage)
    {
        current_health = MathF.Max(current_health - damage, 0);
        EmitSignal(SignalName.HealthReduced);
        //此时碰撞正在发生，不允许改变节点状态（增删节点）。因此延迟删除节点。类似unity协程？是一种异步操作
        CallDeferred(nameof(CheckDied));
    }
    public float GetHealthPersent()
    {
        var persent=Math.Min(current_health/health,1); 
        return persent;
    }
    private void CheckDied()
    {
        if (current_health <= 0)
        {
            EmitSignal(SignalName.Died);     
            if (Owner is Player)
            {
                Player.Instance.GetNode<AnimatedSprite2D>("PlayerAnimation").Play("Died");
            }
            else
            {              
                Owner.QueueFree();
            }
        }
    }
}
