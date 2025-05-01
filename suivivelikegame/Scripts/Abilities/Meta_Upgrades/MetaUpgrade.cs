using Godot;
using System;

public partial class MetaUpgrade : Resource
{
    [Export] public string id;
    [Export] public int max_quantity;
    [Export] public int currency_cost;
    [Export] public string title;
    [Export(Godot.PropertyHint.MultilineText)] public string description;
}
