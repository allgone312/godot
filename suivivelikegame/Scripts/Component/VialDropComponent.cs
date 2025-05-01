using Godot;
using System;

public partial class VialDropComponent : Node
{
    [Export(hint: Godot.PropertyHint.Range, hintString: "0,1")] public float drop_persent = 0.5f; //设置一个掉落率，取值范围0-1
    [Export] HealthComponent healthComponent;
    [Export] PackedScene vialScene;

    public override void _Ready()
    {
        if (healthComponent == null) return;
        healthComponent.Died += OnDied;

        AdjustDropPersent();
    } 
    //监听Died事件，若发生，则创建一个经验瓶节点
    private void OnDied()
    {
        if (GD.Randf() > drop_persent) return; //掉落率控制
        if(!(Owner is Node2D)) return;
        var experienceVial = vialScene.Instantiate() as ExperienceVial;
        experienceVial.GlobalPosition = (Owner as Node2D).GlobalPosition;
        Owner.GetParent().AddChild(experienceVial);
    }

    private void AdjustDropPersent()
    {
        var quantity = MetaProgression.Instance.GetUpgradeQuantityById("experience_gain");
        drop_persent += quantity * 0.1f;
    }
}
