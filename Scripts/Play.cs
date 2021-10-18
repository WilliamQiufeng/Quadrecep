using Godot;
using Quadrecep.Map;
using static Godot.Vector2;

public class Play : Node2D
{
    private ImageTexture _backgroundTexture;
    private AudioStream _stream;
    public Map Map;

    public string MapFile;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Map = new Map("Test");
        Map.ReadMap();
        Map.MapSet.BuildPaths();
        PlaceNotesInScene();
        ((Label) GetNode(new NodePath("HUD/Name"))).Text = Map.MapSet.Name;
        LoadBackground();
        LoadAudio();
    }

    private void PlaceNotesInScene()
    {
        var noteSpriteScene = GD.Load<PackedScene>("res://Scenes/Note.tscn");
        var pathScene = GD.Load<PackedScene>("res://Scenes/Path.tscn");
        // var mapContainer = GetNode<CanvasLayer>("Map");
        foreach (var path in Map.GetMap(0).Paths)
        {
            if (path.TargetNote != null)
            {
                if (noteSpriteScene.Instance() is Node2D noteSprite)
                {
                    noteSprite.GlobalPosition = path.EndPosition + new Vector2(512, 300);
                    var targetNoteDirection = (DirectionObject) path.TargetNote.Direction;
                    noteSprite.Rotation = Zero.AngleToPoint(targetNoteDirection.NetDirection) - Mathf.Pi / 2;
                    noteSprite.GetNode<Node2D>("Side").Visible = targetNoteDirection.HasSide();
                    GetNode("Notes").AddChild(noteSprite);
                }
            }
            else
            {
                // Remove or otherwise. This is only used for debugging.
                if (pathScene.Instance() is Node2D pathSprite)
                {
                    pathSprite.GlobalPosition = path.EndPosition + new Vector2(512, 300);
                    GetNode("Notes").AddChild(pathSprite);
                }
            }

            GD.Print(path);
        }
    }

    private void LoadAudio()
    {
        var audioFile = new File();
        audioFile.Open(MapPath(Map.MapSet.AudioPath), File.ModeFlags.Read);
        var buffer = audioFile.GetBuffer((int) audioFile.GetLen());
        if (Map.MapSet.AudioPath.EndsWith(".mp3"))
        {
            var mp3Stream = new AudioStreamMP3();
            mp3Stream.Data = buffer;
            _stream = mp3Stream;
        }
        else if (Map.MapSet.AudioPath.EndsWith(".wav"))
        {
            var wavStream = new AudioStreamSample();
            wavStream.Data = buffer;
            _stream = wavStream;
        }
        else if (Map.MapSet.AudioPath.EndsWith(".ogg"))
        {
            var oggStream = new AudioStreamOGGVorbis();
            oggStream.Data = buffer;
            _stream = oggStream;
        }

        var audioPlayer = (AudioStreamPlayer) GetNode(new NodePath("ParallaxBackground/AudioStreamPlayer"));
        audioPlayer.Stream = _stream;
        audioPlayer.Playing = true;
    }


    private void LoadBackground()
    {
        var img = new Image();
        var imgPath = MapPath(Map.MapSet.BackgroundPath);
        GD.Print($"Loading background from {imgPath}");
        img.Load(imgPath);
        _backgroundTexture = new ImageTexture();
        _backgroundTexture.CreateFromImage(img);
        var bg = (TextureRect) GetNode(new NodePath("ParallaxBackground/Background"));
        bg.Texture = _backgroundTexture;
        bg.Visible = true;
    }

    private string MapPath(string mapRelativePath)
    {
        return $"user://{Map.MapFile}/{mapRelativePath}";
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}