using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/*
 技能升级管理器
 */
public partial class UpgradeManager : Node
{
    [Export] public Ability_Upgrades[] ability_pool;
    [Export] public ExperienceManager experience_manager;
    [Export] public PackedScene upgradeCardsScene;

    private Dictionary<string, Ability_Upgrades> current_upgrade = new Dictionary<string, Ability_Upgrades>();//已升级的技能
    private Dictionary<string,int> current_upgrade_count = new Dictionary<string,int>(); //已升级技能的升级次数

    private Random random = new Random();
    private Ability_Upgrades[] current_ability_pool; //当前可用于选择的技能池。
    private WeightedTable weighted_ability_pool = new WeightedTable();

    public override void _Ready()
    {
        if (ability_pool == null) { GD.Print("UpgradeManager未给ability_pool赋值"); return; }       
        if (experience_manager==null)
        {
            GD.Print("UpgradeManager.cs未给ExperienceManager赋值");
            return;
        }
        experience_manager.LevelUp += OnLevelUp;

        //剔除无主体技能的升级项,加入权重表
        current_ability_pool = ability_pool.Where(e => string.IsNullOrEmpty(e.parent_ability_id) == true).ToArray();
        foreach(var item in current_ability_pool)
        {
            weighted_ability_pool.AddItem(item.id, item.weight);
        }
    }
    //随机选取升级时弹出的技能选项
    private Ability_Upgrades[] PickUpgrades()
    {
        WeightedTable filtered_weight_pool = new WeightedTable(weighted_ability_pool._Items, weighted_ability_pool._weight_sum); //权重表副本
        Ability_Upgrades[] chosen_upgrades = new Ability_Upgrades[3]; //设置每次出三个升级项
        for (int i = 0; i < 3; i++)
        {
            var ability_key = filtered_weight_pool.PickItem(); //每次升级随机从技能池选出技能升级选项
            chosen_upgrades[i] = current_ability_pool.FirstOrDefault(e => e.id == ability_key);
            filtered_weight_pool.RemoveItem(ability_key); //从权重表移除
            if (filtered_weight_pool._Items.Count == 0) { break; }
        }
        return chosen_upgrades;
    }
    
    //应用选中的技能升级项
    public void ApplyUpgrade(Ability_Upgrades upgrade)
    {
        if (current_upgrade.ContainsKey(upgrade.id))
        {
            current_upgrade_count[upgrade.id]++;
        }
        else
        {
            current_upgrade.Add(upgrade.id, upgrade);
            current_upgrade_count.Add(upgrade.id, 1);
        }        

        //判断是否达到了最大升级次数，若是从池子中移除。max_count=0表示可无限升级
        if(upgrade.max_count > 0)
        {
            if (upgrade.max_count == current_upgrade_count[upgrade.id])
            {
                current_ability_pool = current_ability_pool.Where(e => e.id != upgrade.id).ToArray();
                weighted_ability_pool.RemoveItem(upgrade.id); //记得权重表中也要删除
            }
        }
        //判断是否是主体技能
        if(upgrade is Ability_Itself)
        {
            var child_abilities = ability_pool.Where(e => e.parent_ability_id == upgrade.id).ToArray(); //找出该主体技能下的所有子升级项
            current_ability_pool = current_ability_pool.Concat(child_abilities).ToArray(); //合并到当前升级池中
            foreach (var child_ability in child_abilities)
            {
                weighted_ability_pool.AddItem(child_ability.id, child_ability.weight); //新升级项加入权重表
            }
        }
        
        //事件中心发出信号
        GameEvents.Instance.Emit_AddedUpgrade(upgrade.id,current_upgrade, current_upgrade_count);
    }


    //当每次升级时，出现技能升级选项
    private void OnLevelUp(float newLevel)
    {        
        var upgrade_cards = upgradeCardsScene.Instantiate() as UpgradeCards;
        AddChild(upgrade_cards);
        upgrade_cards.Set_UpgradeCard(PickUpgrades());
        //接收选择技能升级信号
        upgrade_cards.SelectedAbility += OnSelectedAbility;
    }
    //应用此次选择的升级
    private void OnSelectedAbility(Ability_Upgrades upgrade)
    {
        ApplyUpgrade(upgrade);
    }
}
