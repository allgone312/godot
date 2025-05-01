using Godot;
using System;
using System.IO;

public partial class DeathComponent : Node2D
{
    [Export] public Texture2D texture;
    [Export] public Node healthComponent;
    [Export] public AudioStream[] deathAudioStreams;

    private Sprite2D sprite;
    private AnimationPlayer animationPlayer;
    private AudioStreamPlayer2D audioStreamPlayer;

    private Vector2 spawn_positon = Vector2.Zero;
    private Random random=new Random();
    public override void _Ready()
    {
        if(texture == null) { GD.Print("DeathComponent.cs未给texture赋值。"); return; }
        if (healthComponent == null) { GD.Print("DeathComponent.cs未给HealthComponent赋值。");return; }

        sprite = GetNode<Sprite2D>("Sprite2D");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        audioStreamPlayer = GetNode<AudioStreamPlayer2D>("DeathAudioStreamPlayer2D");
        sprite.Texture = texture;
        sprite.Modulate = new Godot.Color(Modulate.R, Modulate.G, Modulate.B, 0.0f);

        var health = healthComponent as HealthComponent;
        health.Died += OnDied;
    }

    private void OnDied()
    {
        if(Owner==null || !(Owner is Node2D)) { return; }
        sprite.Modulate = new Godot.Color(Modulate.R, Modulate.G, Modulate.B, 0.0f);
        spawn_positon = ((Node2D)Owner).GlobalPosition;
        var entities = GetTree().GetFirstNodeInGroup("Entities_Layer");
        GlobalPosition = spawn_positon;
        Reparent(entities);     
        if (Player.Instance == null) { return; }
        var dir = Player.Instance.GlobalPosition - GlobalPosition;
        if (dir.X < 0)
        {
            sprite.FlipH=true;
        }
        else
        {
            sprite.FlipH = false;
        }
        animationPlayer.Play("died");

        //播放死亡音效
        var ran_index = random.Next(deathAudioStreams.Length);
        audioStreamPlayer.Stream = deathAudioStreams[ran_index];
        audioStreamPlayer.Play();
    }
}
