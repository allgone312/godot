using Godot;
using System;
using Godot.Collections;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

public partial class MetaProgression : Node
{
    private static MetaProgression _instance;
    public static MetaProgression Instance { get { return _instance; } }
    public override void _EnterTree()
    {
        if (_instance != null)
        {
            // 如果已存在实例，销毁自身
            QueueFree();
            return;
        }
        _instance = this;
    }

    // 释放时清理静态引用
    public override void _ExitTree()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private const string SAVE_FILE_PATH = "user://game.save";
    private const string TMP_SAVE_FILE_PATH = "user://game.tmp";  //临时保存文件

    private SaveData saveData=new();

    public override void _Ready()
    {
        LoadFromSavefile();

        GameEvents.Instance.ExperienceVialCollected += OnExperienceCollected;
    }

    public void LoadFromSavefile()
    {
        if (!FileAccess.FileExists(SAVE_FILE_PATH)) { return; }
        var file = FileAccess.Open(SAVE_FILE_PATH, FileAccess.ModeFlags.Read);
        string json=file.GetAsText();
        file.Close();
        //解析json
        var jsonResult=Json.ParseString(json);
        var data=jsonResult.AsGodotDictionary();
        if(data==null) { return; }

        saveData = new SaveData() {
            meta_upgrade_currency = (int)data["meta_upgrade_currency"],
            meta_upgrades = data["meta_upgrades"].As<Dictionary<string, int>>()
        };
    }

    public void SaveToSavefile()
    {
        string json = Json.Stringify(saveData.GetSaveDict());
        var file = FileAccess.Open(TMP_SAVE_FILE_PATH, FileAccess.ModeFlags.Write);
        file.StoreString(json);
        file.Close();
        DirAccess.RenameAbsolute(TMP_SAVE_FILE_PATH, SAVE_FILE_PATH);  //将临时文件重命名，保证每次储存的原子性
    }

    public void AddMetaUpgrade(MetaUpgrade upgrade)
    {
        var quantity = saveData.GetMetaUpgrades(upgrade.id);
        quantity += 1;
        saveData.SetMetaUpgrades(upgrade.id, quantity);
    }

    public void UpdateMetaUpgradeCurrency(int number)
    {
        saveData.meta_upgrade_currency += (int)number;
    }

    public int GetCurrency()
    {
        return saveData.meta_upgrade_currency;
    }
    public int GetUpgradeQuantityById(string id)
    {
        if (saveData.meta_upgrades.ContainsKey(id))
        {
            return saveData.GetMetaUpgrades(id);
        }
        return 0;
    }

    private void OnExperienceCollected(float number)
    {
        saveData.meta_upgrade_currency += (int)number;
    }
}
