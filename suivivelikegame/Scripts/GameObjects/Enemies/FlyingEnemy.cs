using Godot;
using System;

public partial class FlyingEnemy : CharacterBody2D
{
    [Export] public float enemy_damage = 1.0f;

    private const float ATTACK_RANGE = 20.0f;

    private Vector2 _direction = Vector2.Zero;
    private AnimationTree animationTree;
    private VelocityComponent velocityComponent;

    public override void _Ready()
    {
        animationTree = GetNode<AnimationTree>("AnimationTree");
        velocityComponent = GetNode<VelocityComponent>("VelocityComponent");

        animationTree.Active = true;
    }
    public override void _PhysicsProcess(double delta)
    {
        velocityComponent.AccelerateToPlayer();
        velocityComponent.Move(this);
        _direction = velocityComponent.GetDirectionToPlayer();
        SetAnimation(_direction);
    }

    //更新动画状态机
    private void SetAnimation(Vector2 dir)
    {
        if (Velocity == Vector2.Zero)
        {
            animationTree.Set("parameters/conditions/attack", false);
            animationTree.Set("parameters/conditions/idle", true);
            animationTree.Set("parameters/conditions/is_walking", false);
        }
        else
        {
            animationTree.Set("parameters/conditions/attack", false);
            animationTree.Set("parameters/conditions/idle", false);
            animationTree.Set("parameters/conditions/is_walking", true);
        }
        if (GlobalPosition.DistanceSquaredTo(Player.Instance.GlobalPosition) < Math.Pow(ATTACK_RANGE, 2))
        {
            animationTree.Set("parameters/conditions/attack", true);
            animationTree.Set("parameters/conditions/idle", false);
            animationTree.Set("parameters/conditions/is_walking", false);
        }

        if (dir != Vector2.Zero)
        {
            var _dir = dir.X > 0 ? 1.0 : -1.0;
            animationTree.Set("parameters/Attack/blend_position", _dir);
            animationTree.Set("parameters/Idle/blend_position", _dir);
            animationTree.Set("parameters/Walk/blend_position", _dir);
        }
    }
}
