using Godot;
using System;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Play : Node2D
{
    public Map Map;
    public string map_file;
    private AudioStream stream;

    private ImageTexture background_texture;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Map = new Map("Test");
        Map.ReadMap();
        ((Label) GetNode(new NodePath("HUD/Name"))).Text = Map.map_set.Name;
        LoadBackground();
        LoadAudio();
    }

    private void LoadAudio()
    {
        var audioFile = new File();
        audioFile.Open(MapPath(Map.map_set.Audio), File.ModeFlags.Read);
        var buffer = audioFile.GetBuffer((int) audioFile.GetLen());
        if (Map.map_set.Audio.EndsWith(".mp3"))
        {
            var mp3Stream = new AudioStreamMP3();
            mp3Stream.Data = buffer;
            stream = mp3Stream;
        }
        else if (Map.map_set.Audio.EndsWith(".wav"))
        {
            var wavStream = new AudioStreamSample();
            wavStream.Data = buffer;
            stream = wavStream;
        }
        else if (Map.map_set.Audio.EndsWith(".ogg"))
        {
            var oggStream = new AudioStreamOGGVorbis();
            oggStream.Data = buffer;
            stream = oggStream;
        }
        var audioPlayer = (AudioStreamPlayer) GetNode(new NodePath("ParallaxBackground/AudioStreamPlayer"));
        audioPlayer.Stream = stream;
        audioPlayer.Playing = true;
    }
    
    

    private void LoadBackground()
    {
        var img = new Image();
        var imgPath = MapPath(Map.map_set.Background);
        GD.Print($"Loading background from {imgPath}");
        img.Load(imgPath);
        background_texture = new ImageTexture();
        background_texture.CreateFromImage(img);
        var bg = (TextureRect) GetNode(new NodePath("ParallaxBackground/Background"));
        bg.Texture = background_texture;
        bg.Visible = true;
    }

    public string MapPath(string mapRelativePath)
    {
        return $"user://{Map.map_file}/{mapRelativePath}";
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
