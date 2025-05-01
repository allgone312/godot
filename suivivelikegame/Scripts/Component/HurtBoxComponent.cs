using Godot;
using System;

public partial class HurtBoxComponent : Area2D
{	
	[Export] public HealthComponent healthComponent;
	[Export] public RandomAudioComponent randomAudioComponent;

    private PackedScene floatingTextScene = ResourceLoader.Load<PackedScene>("uid://dgs75cuh45nmp");
	
	public override void _Ready()
	{
		if (healthComponent == null) return;
		AreaEntered += OnAreaEntered;
    }

	public void OnAreaEntered(Area2D area2D)
	{
		if(!(area2D is HitBoxComponent)) return;
		var hitBoxComponent = (HitBoxComponent) area2D;
        healthComponent.Damage(hitBoxComponent.damage);

		//漂浮显示伤害数字
		var floating_text = floatingTextScene.Instantiate() as FloatingDamage;
        floating_text.GlobalPosition = GlobalPosition;
        GetTree().GetFirstNodeInGroup("Foreground_Layer").AddChild(floating_text);		
		floating_text.DisplayFloatingDamage(Math.Floor(hitBoxComponent.damage).ToString());

		//播放击中音效
		randomAudioComponent.RandomPlay();
    }
}
