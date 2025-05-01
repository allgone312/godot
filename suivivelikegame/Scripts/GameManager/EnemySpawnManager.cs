using Godot;
using System;
using System.Linq;
using System.Reflection;

public partial class EnemySpawnManager : Node
{
    [Export] public EnemyProperty[] enemyProperties;   
    [Export] public GameTimeManager gameTimeManager;

    private const float SPAWN_RANGE = 200.0f;
    private const float SPAWN_INTERVAL = 2.0f;
    private Node entities_layer;
    private Random _random = new Random();
    private Timer _timer;
    private Vector2 spawnPosition = Vector2.Zero;
    private WeightedTable enemyWeightTable = new WeightedTable();

    private string[] enemyNames; //敌人key
    private int[] enemyWeights;  //敌人重生权重
    private int[] difficultyToSpawn;  //敌人开始出现的难度
    int min_difficultyToSpawn = 0;
    int max_difficultyToSpawn = 0;
    int spawned_count = 1;

    private float difficulty;
    public override void _Ready()
    {
        enemyNames=enemyProperties.Select(e=>e.enemyName).ToArray();
        enemyWeights =enemyProperties.Select(e=>e.weight).ToArray();
        difficultyToSpawn = enemyProperties.Select(e => e.difficultyToSpawn).ToArray();

        if (gameTimeManager == null) { GD.Print("EnemySpawnManager.cs未给gameTimeManager赋值");return; }
        gameTimeManager.DifficultyIncreased += OnDifficultyIncreased;
        entities_layer = GetTree().GetFirstNodeInGroup("Entities_Layer");
        _timer = GetNode<Timer>("Timer");
        _timer.Timeout += OnTimeOut;

        enemyWeightTable.AddItem(enemyNames[0], enemyWeights[0]); //将初始怪物先添加进权重表,其difficultyToSpawn记得设为0
        min_difficultyToSpawn = difficultyToSpawn.Where(e => e > 0).Min();
        max_difficultyToSpawn = difficultyToSpawn.Max();
    }
    private Vector2 GetSpawnPosition() 
    {
        var rotate = Convert.ToSingle(_random.NextDouble() * Math.PI * 2);      
        for (int i = 0; i < 4; i++)
        {
            var randomDir = Vector2.Right.Rotated(rotate);
            spawnPosition = Player.Instance.GlobalPosition + (randomDir * SPAWN_RANGE);
            var additionalCheckOffset = randomDir * 20; //防止怪生成时卡墙上

            //由player向spawanPostion设置一条射线，若此条射线穿过了边界碰撞体，则将spawnposition旋转90度再次尝试。最多尝试4次
            var query_paramaters = PhysicsRayQueryParameters2D.Create(Player.Instance.GlobalPosition, spawnPosition + additionalCheckOffset, 1);
            var result = GetTree().Root.World2D.DirectSpaceState.IntersectRay(query_paramaters);
            if(result.Count > 0)
            {
                rotate += Mathf.DegToRad(90);
            }
            else
            {
                break;
            }
        }
        
        return spawnPosition;
    }

    private void OnDifficultyIncreased(float current_difficulty)
    {
        difficulty = current_difficulty;
        var time_off = (0.2 / 12) * current_difficulty; //一分钟内使敌人生成间隔减少0.2秒
        time_off = Math.Min(time_off, 0.5); //最多减少0.5秒
        _timer.WaitTime = SPAWN_INTERVAL - time_off;

        //随难度增加，设置不同敌人生成权重到权重表中        
        int index = 0;
        
        if (current_difficulty >= min_difficultyToSpawn && spawned_count < difficultyToSpawn.Length)
        {            
            index = Array.IndexOf(difficultyToSpawn, min_difficultyToSpawn);
            enemyWeightTable.AddItem(enemyNames[index], enemyWeights[index]);
            if (min_difficultyToSpawn < max_difficultyToSpawn)
            {
                min_difficultyToSpawn = difficultyToSpawn.Where(e => e > min_difficultyToSpawn).Min();
            }            
            spawned_count++;
        }
    }

    private void OnTimeOut()
    {

        for(int i = 0; i < (difficulty+10) / 10; i++)  //每增加10难度，每次多生成一只怪物
        {
            var enemy_key = enemyWeightTable.PickItem();
            int index = Array.IndexOf(enemyNames, enemy_key);

            var newEnemy = enemyProperties[index].enemyPackedScene.Instantiate() as Node2D;
            newEnemy.GlobalPosition = GetSpawnPosition();
            entities_layer.AddChild(newEnemy);
        }
        
        _timer.Start();
    }
}
