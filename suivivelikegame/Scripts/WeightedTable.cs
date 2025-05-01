using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class WeightedTable : Node
{
    private Dictionary<string, int> items;
    private int weight_sum = 0; //总权重
    public Dictionary<string, int> _Items { get { return items; } set { items = value; } }
    public int _weight_sum { get { return weight_sum; } set { weight_sum = value; } }

    public WeightedTable() 
    {
        _Items =  [];
        _weight_sum = 0;
     }
    public WeightedTable(Dictionary<string, int> items, int weight_sum)
    {
        _Items = new Dictionary<string, int>(items);
        _weight_sum = weight_sum;
    }


    public void AddItem(string item,int weight)
    {
        weight_sum += weight;
        items.Add(item, weight);       
    }
    public void RemoveItem(string item) 
    {
        weight_sum -= items[item];
        items.Remove(item);        
    }

    public string PickItem()
    {
        var chonsen_weight = GD.RandRange(1, weight_sum);
        int iteration_sum = 0;
        foreach (var item in items)
        {
            iteration_sum += item.Value;
            if (chonsen_weight <= iteration_sum)
            {
                return item.Key;
            }
        }
        return null;
    }
}
