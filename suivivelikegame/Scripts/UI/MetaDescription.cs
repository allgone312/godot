using Godot;
using System;
using System.Linq;

public partial class MetaDescription : PanelContainer
{
    [Signal] public delegate void MetaPurchasedEventHandler();

    private Label nameLabel;
    private Label descriptionLabel;
    private Label levelLabel;
    private Slider progressSlider;
    private Label costLabel;
    private Button purchaseButton;
    private TextureRect texture;

    private MetaUpgrade chosen_upgrade;
    
    private int cost = 0;
    private bool disable = false; //标识当前是否可用状态，防止玩家多次点击卡片
    public override void _Ready()
    {
        nameLabel = GetNode<Label>("MarginContainer/VBoxContainer/PanelContainer/NameLabel");
        descriptionLabel = GetNode<Label>("MarginContainer/VBoxContainer/DescriptionLabel");
        levelLabel = GetNode<Label>("MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/LevelLabel");
        progressSlider = GetNode<Slider>("MarginContainer/VBoxContainer/MarginContainer/VBoxContainer/ProgressSlider");
        costLabel = GetNode<Label>("MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer/CostLabel");
        purchaseButton = GetNode<Button>("MarginContainer/VBoxContainer/VBoxContainer/PurchaseButton");
        texture = GetNode<TextureRect>("MarginContainer/VBoxContainer/VBoxContainer/HBoxContainer/TextureRect");

        purchaseButton.Pressed += OnPurchasePressed;
    }

    public void Set_MetaDescription(MetaUpgrade upgrade)
    {
        chosen_upgrade = upgrade;
        nameLabel.Text = upgrade.title;
        descriptionLabel.Text = upgrade.description;
        UpdateProgress();                
    }

    private void UpdateProgress()
    {
        int quentity = MetaProgression.Instance.GetUpgradeQuantityById(chosen_upgrade.id);
        
        cost = chosen_upgrade.currency_cost * (quentity+1);
        progressSlider.Value = (float)quentity / chosen_upgrade.max_quantity;
        if(quentity == chosen_upgrade.max_quantity)
        {
            levelLabel.Text = "Lv.MAX" ;
            costLabel.Text = "已达最大等级";
            texture.Visible = false;
        }
        else
        {
            levelLabel.Text = "Lv." + quentity.ToString();
            costLabel.Text = cost.ToString();
        }
        
        var currency = MetaProgression.Instance.GetCurrency();
        if(currency< cost || quentity == chosen_upgrade.max_quantity)
        {
            purchaseButton.Disabled = true;
        }
        else 
        { 
            purchaseButton.Disabled = false; 
        }
    }

    private void OnPurchasePressed()
    {        
        MetaProgression.Instance.UpdateMetaUpgradeCurrency(-cost);
        MetaProgression.Instance.AddMetaUpgrade(chosen_upgrade);

        GetTree().CallGroup("meta_upgrade_cards", "UpdateProgress");  //在购买后对全部卡片进行更新。记得设置MetaDescription的分组

        EmitSignal(SignalName.MetaPurchased); //用于更新右上角余额的显示
    }
}
