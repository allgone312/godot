using Godot;
using System;
using Godot.Collections;

public partial class SaveData : Resource
{
    private int _meta_upgrade_currency; //局外升级用点数
    private Dictionary<string, int> _meta_upgrades = new(); //局外升级项

    public int meta_upgrade_currency {
        get { return _meta_upgrade_currency; }
        set { _meta_upgrade_currency = value < 0 ? 0 : value; }
    }
    public Dictionary<string, int> meta_upgrades{
        get => new Dictionary<string, int>(_meta_upgrades); // 返回副本防止外部直接修改
        set
        {
            if (_meta_upgrades != value)
            {
                _meta_upgrades = value ?? new Dictionary<string, int>();
            }
        }
    }

    public int GetMetaUpgrades(string key) {
        if (!_meta_upgrades.ContainsKey(key)) { return 0; }
        return _meta_upgrades[key];
    }
    public void SetMetaUpgrades(string key, int value) {
        if (_meta_upgrades.ContainsKey(key)) { _meta_upgrades[key] = value; }
        else { _meta_upgrades.Add(key, value); }
    }
    public Dictionary GetSaveDict()
    {
        var data = new Dictionary
        {
            { "meta_upgrade_currency" , _meta_upgrade_currency},
            { "meta_upgrades" , _meta_upgrades}
        };
        return data;
    }
}
