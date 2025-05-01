using Godot;
using System;
using System.Drawing;
using System.Linq;

public partial class CardDescription : PanelContainer
{
    [Signal] public delegate void SelectedEventHandler(Ability_Upgrades upgrade);

    private TextureRect icon;
	private Label nameLabel;
	private Label descriptionLabel;
    private AnimationPlayer animePlayer;
    private UiAudioPlayerComponent hoverSound;

    private Ability_Upgrades chosen_upgrade;
    private bool disable = false; //标识当前是否可用状态，防止玩家多次点击卡片
    public override void _Ready()
    {
        icon = GetNode<TextureRect>("MarginContainer/VBoxContainer/Control/Icon");
        nameLabel= GetNode<Label>("MarginContainer/VBoxContainer/PanelContainer/NameLabel");
        descriptionLabel= GetNode<Label>("MarginContainer/VBoxContainer/DescriptionLabel");
        animePlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        hoverSound = GetNode<UiAudioPlayerComponent>("Hoversound");
        GuiInput += OnGuiInput;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }    

    public void Set_AbilityDescription(Ability_Upgrades upgrade)
    {        
        nameLabel.Text=upgrade.name;
        descriptionLabel.Text=upgrade.description;
        icon.Texture = upgrade.icon;
        chosen_upgrade =upgrade;     
    }
    public async void PlayAnimeDisplay(float delay)
    {
        Modulate = new Godot.Color(Modulate.R, Modulate.G, Modulate.B, 0.0f);
        await ToSignal(GetTree().CreateTimer(delay), SceneTreeTimer.SignalName.Timeout);
        Modulate = new Godot.Color(Modulate.R, Modulate.G, Modulate.B, 1.0f);
        animePlayer.Play("display");
    }
    private void PlayAnimeSelected()
    {
        animePlayer.Play("selected");
    }
    private void PlayAnimeDiscard()
    {
        animePlayer.Play("discard");
    }

    //监听鼠标点击事件，若选择了某一项升级发出信号。信号向上传递给UpgradeCards
    public async void OnGuiInput(InputEvent input)
    {
        if(disable) return;
        if (input.IsActionPressed("mouse_left_click"))
        {
            disable = true;
            
            var card_nodes = GetTree().GetNodesInGroup("upgrade_cards").OfType<CardDescription>().ToArray();
            foreach (var item in card_nodes)
            {
                if (item == this) continue;             
                item.PlayAnimeDiscard();
            }
            await ToSignal(GetTree().CreateTimer(0.2f), SceneTreeTimer.SignalName.Timeout);
            PlayAnimeSelected();
            await ToSignal(animePlayer, AnimationPlayer.SignalName.AnimationFinished); //直接使用信号语句，动画全部播放完毕再发出信号
            EmitSignal(SignalName.Selected,chosen_upgrade);
        }
    }
    private void OnMouseEntered()
    {
        if (disable) return;

        hoverSound.RandomPlay();
        Tween tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.One * 1.2f, 0.2)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
    }
    private void OnMouseExited()
    {
        if (disable) return;
        Tween tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.One * 1.0f, 0.2)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
    }
}
