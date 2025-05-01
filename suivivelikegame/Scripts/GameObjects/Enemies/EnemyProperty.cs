using Godot;
using System;

public partial class EnemyProperty : Resource
{
    /*对应的敌人节点*/
    [Export] public PackedScene enemyPackedScene;
    /*敌人key*/
    [Export] public string enemyName; 
    /*敌人类型*/
    [Export] public string enemyType;
    /*每个敌人权重*/
    [Export] public int weight;
    /*敌人开始出现的难度*/
    [Export] public int difficultyToSpawn;  
}
