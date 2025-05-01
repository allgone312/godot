using Godot;
using System;

public partial class AxeAbility : Node2D
{
	private const float MAX_RADIUS = 100.0f;
	private Tween tween;
	Vector2 base_rotation = Vector2.Right;

    /*Tween 主要用于需要将一个数值属性插值到一系列值的动画。可以在其中指定 关键帧，然后计算机会插入出现在它们之间的帧。使用 Tween 制作动画被称为补间动画。*/
    public override void _Ready()
	{
		base_rotation = Vector2.Right.Rotated((float)GD.RandRange(0, Math.PI * 2));
		tween = CreateTween();
        tween.TweenMethod(Callable.From<float>(Tween_Func), 0.0, 2.5, 3); //表示float rotation在3秒内从0.0增长到2.5。注意前两个参数是浮点数。
        tween.TweenCallback(Callable.From(QueueFree)); //完成后释放该节点
	}

	private void Tween_Func(float rotation)
	{
		var percent = rotation / 2;
		var current_radius=percent * MAX_RADIUS;
		var current_direction = base_rotation.Rotated(rotation * (float)Math.PI * 2);
		//注意对player为空的判断。如果在飞行过程中玩家死亡，则玩家节点会清掉，可能导致报错
		if(Player.Instance==null)
		{
			return;
        }
		GlobalPosition = Player.Instance.GlobalPosition + (current_direction * current_radius);
    }
}
