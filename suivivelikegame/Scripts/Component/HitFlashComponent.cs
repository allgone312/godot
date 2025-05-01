using Godot;
using System;

public partial class HitFlashComponent : Node
{
    [Export] public HealthComponent healthComponent;
    [Export] public AnimatedSprite2D sprite;
    [Export] public ShaderMaterial hit_flash_material;

    private Tween hitFlashTween;
    public override void _Ready()
    {
        healthComponent.HealthReduced += OnHealthChanged;
        sprite.Material = hit_flash_material;
    }

    private void OnHealthChanged()
    {
        if(hitFlashTween!=null && hitFlashTween.IsValid())
        {
            hitFlashTween.Kill();
        }

        (sprite.Material as ShaderMaterial).SetShaderParameter("lerp_persent", 1.0);
        hitFlashTween = CreateTween();
        hitFlashTween.TweenProperty(sprite.Material, "shader_parameter/lerp_persent", 0.0, 0.25)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
    }
}
