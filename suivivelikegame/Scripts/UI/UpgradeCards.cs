using Godot;
using System;

public partial class UpgradeCards : CanvasLayer
{
	[Export] PackedScene cardDescriptionScene;

	[Signal] public delegate void SelectedAbilityEventHandler(Ability_Upgrades upgrade);

	private HBoxContainer card_container;
	private AnimationPlayer player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		card_container = GetNode<HBoxContainer>("MarginContainer/CardContainer");
		player = GetNode<AnimationPlayer>("AnimationPlayer");

        //当升级后弹出升级选项，其他节点暂停。注意在检查器中将此节点process->设置为always，表示其他节点暂停时该节点依然运行。
        GetTree().Paused = true; 
	}
	//对传入的升级池进行处理。用于显示多个可升级选项
	public void Set_UpgradeCard(Ability_Upgrades[] upgrades)
	{
		var delay = 0.0f;
		foreach(var u in upgrades)
		{
			var cardDescription=cardDescriptionScene.Instantiate() as CardDescription;			
			card_container.AddChild(cardDescription);
            cardDescription.Set_AbilityDescription(u);
			cardDescription.PlayAnimeDisplay(delay); //按次序显示的动画效果
            //监听选择升级信号
            cardDescription.Selected += OnSelected;
			delay += 0.2f;  //每张卡显示出来有0.2秒延迟
        }
	}

	private async void OnSelected(Ability_Upgrades upgrade)
	{
		//继续将信号向上传递给UpgradeManager。职责单一原则。
		EmitSignal(SignalName.SelectedAbility, upgrade);

		player.Play("Out");
		await ToSignal(player, AnimationPlayer.SignalName.AnimationFinished);
        GetTree().Paused = false;
		QueueFree();
    }
}
