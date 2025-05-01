using Godot;
using System;

public partial class MetaMenu : CanvasLayer
{
	[Export] public MetaUpgrade[] upgrades;

	[Signal] public delegate void BackButtonPressedEventHandler();
    private PackedScene metaDescriptionCardScene;

	private Button backButton;
    private GridContainer grid;
	private Label currencyLabel;
	public override void _Ready()
	{
		metaDescriptionCardScene = ResourceLoader.Load<PackedScene>("uid://34jfwdvhi51n");

		backButton = GetNode<Button>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/BackButton");
        grid = GetNode<GridContainer>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/ScrollContainer/GridContainer");
		currencyLabel = GetNode<Label>("MarginContainer/PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/HBoxContainer/CurrencyLabel");

		backButton.Pressed += OnBackPressed;

		currencyLabel.Text=MetaProgression.Instance.GetCurrency().ToString();
        foreach (var upgrade in upgrades)
		{
			var metaDescriptionCard = metaDescriptionCardScene.Instantiate() as MetaDescription;
			grid.AddChild(metaDescriptionCard);
			metaDescriptionCard.Set_MetaDescription(upgrade);

			metaDescriptionCard.MetaPurchased += OnMetaPurchased;
        }
	}

	private async void OnBackPressed()
	{
        //引入转场动画
        ScreenTransition.Instance.Transition();
        MetaProgression.Instance.SaveToSavefile();
        await ToSignal(ScreenTransition.Instance, ScreenTransition.SignalName.HalfScreenTransition);
		
        EmitSignal(SignalName.BackButtonPressed);
	}
	private void OnMetaPurchased()
	{
        currencyLabel.Text = MetaProgression.Instance.GetCurrency().ToString();
    }
}
