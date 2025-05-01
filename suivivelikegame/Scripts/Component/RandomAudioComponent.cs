using Godot;
using System;

public partial class RandomAudioComponent : AudioStreamPlayer2D
{
    [Export] public AudioStream[] audioStreams;
    [Export] public bool random_picth = true; //为随机播放的如击中音效设置不同音高，增加更多变化
    [Export] public float max_pitch = 1.1f;
    [Export] public float min_pitch = 0.9f;

    private Random random=new Random();

    public void RandomPlay()
    {
        if (audioStreams == null || audioStreams.Length == 0) return;
        var ran_index=random.Next(audioStreams.Length);

        if (random_picth)
        {
            PitchScale = (float)GD.RandRange(min_pitch, max_pitch);
        }
        else
        {
            PitchScale = 1.0f;
        }

        Stream = audioStreams[ran_index];
        Play();
    }

}
