using Godot;
using System;

public partial class VelocityComponent : Node
{
    [Export] public float max_speed = 40.0f;
    [Export] public float acceleration = 5.0f; //加速度:值越小重新追击玩家的时间越长，如飞行怪物

    private Vector2 velocity = Vector2.Zero;
    
    //敌人向Player的移动方法
    public void AccelerateToPlayer()
    {
        var owner_node2d = Owner as Node2D;
        if(owner_node2d == null ) { return; }
        if (Player.Instance == null) { return;}

        var direction = (Player.Instance.GlobalPosition - owner_node2d.GlobalPosition).Normalized();
        AccelerateInDirection(direction);
    }
    public void AccelerateInDirection(Vector2 direction)
    {
        var desired_velocity = direction * max_speed;
        //设置速度插值，模拟移动加速及减速效果
        velocity = velocity.Lerp(desired_velocity, (float)(1 - Math.Exp(-acceleration * GetProcessDeltaTime())));
    }
    public void Move(CharacterBody2D characterBody)
    {
        characterBody.Velocity=velocity;
        characterBody.MoveAndSlide();
        velocity = characterBody.Velocity;
    }
    public Vector2 GetDirectionToPlayer()
    {
        var owner_node2d = Owner as Node2D;
        if (owner_node2d == null) { return Vector2.Zero; }
        if (Player.Instance == null) { return Vector2.Zero; }

        var direction = (Player.Instance.GlobalPosition - owner_node2d.GlobalPosition).Normalized();
        return direction;
    }
}
