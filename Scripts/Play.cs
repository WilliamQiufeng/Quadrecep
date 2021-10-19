using Godot;
using Quadrecep.Map;
using static Godot.Vector2;
using Path = Quadrecep.Map.Path;

public class Play : Node2D
{
    private ImageTexture _backgroundTexture;
    private AudioStream _stream;
    private Map _map;
    private MapObject _mapObject;
    public float Time;

    public string MapFile;
    private int _pathIndex = 0;
    public Path CurrentPath => _mapObject.Paths[_pathIndex];

    [Export(PropertyHint.Range, "0,10,1")] private int _mapIndex;

    [Export] private string _mapName;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LoadMap();
        PlaceNotesInScene();
        GetNode<Label>("HUD/Name").Text = _map.MapSet.Name;
        LoadBackground();
        LoadAudio();
    }

    private void LoadMap()
    {
        _map = new Map(_mapName);
        _map.ReadMap();
        _mapObject = _map.GetMap(_mapIndex);
        _mapObject.BuildPaths();
    }

    public override void _Process(float delta)
    {
        UpdateTime();
        UpdateCurrentPath();
    }

    private void UpdateCurrentPath()
    {
        while (CurrentPath.EndTime < Time)
        {
            _pathIndex++;
            GD.Print(CurrentPath);
        }
    }

    private void UpdateTime()
    {
        Time = (float) (GetNode<AudioStreamPlayer>("AudioStreamPlayer").GetPlaybackPosition() +
            AudioServer.GetTimeSinceLastMix() - AudioServer.GetOutputLatency()) * 1000;
    }

    private void PlaceNotesInScene()
    {
        var noteSpriteScene = GD.Load<PackedScene>("res://Scenes/Note.tscn");
        var pathScene = GD.Load<PackedScene>("res://Scenes/Path.tscn");
        // var mapContainer = GetNode<CanvasLayer>("Map");
        foreach (var path in _mapObject.Paths)
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
        audioFile.Open(MapPath(_map.MapSet.AudioPath), File.ModeFlags.Read);
        var buffer = audioFile.GetBuffer((int) audioFile.GetLen());
        if (_map.MapSet.AudioPath.EndsWith(".mp3"))
        {
            var mp3Stream = new AudioStreamMP3();
            mp3Stream.Data = buffer;
            _stream = mp3Stream;
        }
        else if (_map.MapSet.AudioPath.EndsWith(".wav"))
        {
            var wavStream = new AudioStreamSample();
            wavStream.Data = buffer;
            _stream = wavStream;
        }
        else if (_map.MapSet.AudioPath.EndsWith(".ogg"))
        {
            var oggStream = new AudioStreamOGGVorbis();
            oggStream.Data = buffer;
            _stream = oggStream;
        }

        var audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        audioPlayer.Stream = _stream;
        audioPlayer.Play();
    }


    private void LoadBackground()
    {
        var img = new Image();
        var imgPath = MapPath(_map.MapSet.BackgroundPath);
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
        return $"user://{_map.MapFile}/{mapRelativePath}";
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}