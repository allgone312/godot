using Godot;
using System;
using static Godot.TextServer;

public partial class RangedAttackComponent : Node
{
    [Export] public float shoot_interval = 3.0f;
    [Export] public PackedScene bulletScene;

    public bool is_can_shoot;
    private Timer timer;    
    private Node foreground_layer;
    public override void _Ready()
    {
        is_can_shoot = true;
        foreground_layer = GetTree().GetFirstNodeInGroup("Foreground_Layer");
        timer = GetNode<Timer>("Timer");
        
        timer.Timeout += OnTimeOut;
    }

    public void ShootBullet(Vector2 direction)
    {
        if (!is_can_shoot) { return; }
        is_can_shoot = false;
        timer.WaitTime = shoot_interval;
        timer.Start();
        var bullet = bulletScene.Instantiate() as BasicBullet;
        bullet.GlobalPosition = ((Node2D)Owner).GlobalPosition;
        foreground_layer.AddChild(bullet);       
        bullet.ShootBullet(direction);
    }

    private void OnTimeOut()
    {
        is_can_shoot = true;
    }
}
