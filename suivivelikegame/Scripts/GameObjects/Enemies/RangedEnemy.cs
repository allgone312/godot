using Godot;
using System;

public partial class RangedEnemy : CharacterBody2D
{
    [Export] public float enemy_damage = 1.0f;

    private const float ATTACK_RANGE = 150.0f;

    private Vector2 _direction = Vector2.Zero;
    private AnimationTree animationTree;
    private VelocityComponent velocityComponent;
    private RangedAttackComponent rangeAttackComponent;

    public override void _Ready()
    {
        animationTree = GetNode<AnimationTree>("AnimationTree");
        velocityComponent = GetNode<VelocityComponent>("VelocityComponent");
        rangeAttackComponent = GetNode<RangedAttackComponent>("RangedAttackComponent");

        animationTree.Active = true;
    }
    public override void _PhysicsProcess(double delta)
    {
        if (GlobalPosition.DistanceSquaredTo(Player.Instance.GlobalPosition) > Math.Pow(ATTACK_RANGE, 2))
        {
            velocityComponent.AccelerateToPlayer();
            velocityComponent.Move(this);
        }
        else
        {
            Velocity = Vector2.Zero;
            MoveAndSlide();
        }
        _direction = velocityComponent.GetDirectionToPlayer();
        SetAnimation(_direction);
    }

    //更新动画状态机
    private void SetAnimation(Vector2 dir)
    {
        if (Velocity==Vector2.Zero)
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
        if (GlobalPosition.DistanceSquaredTo(Player.Instance.GlobalPosition) <= Math.Pow(ATTACK_RANGE, 2) && rangeAttackComponent.is_can_shoot)
        {
            animationTree.Set("parameters/conditions/attack", true);
            animationTree.Set("parameters/conditions/idle", false);
            animationTree.Set("parameters/conditions/is_walking", false);
            RangedAttack(dir);
        }
        if (dir != Vector2.Zero)
        {
            var _dir = dir.X > 0 ? 1.0 : -1.0;
            animationTree.Set("parameters/Attack/blend_position", _dir);
            animationTree.Set("parameters/Idle/blend_position", _dir);
            animationTree.Set("parameters/Walk/blend_position", _dir);
        }
    }

    private void RangedAttack(Vector2 direction)
    {
         rangeAttackComponent.ShootBullet(direction);
    }
}
