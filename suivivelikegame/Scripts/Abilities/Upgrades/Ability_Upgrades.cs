using Godot;
using System;

public partial class Ability_Upgrades : Resource
{   
    [Export] public string id;
    [Export] public int weight;
    [Export] public string parent_ability_id;
    [Export] public int max_count;
    [Export] public string name;
    [Export] public Texture2D icon;
    [Export(Godot.PropertyHint.MultilineText)] public string description;    
}
