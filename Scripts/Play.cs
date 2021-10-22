using System;
using Godot;
using Quadrecep.Gameplay;
using Quadrecep.Map;
using static Godot.Vector2;
using Path = Quadrecep.Map.Path;

namespace Quadrecep
{
    public class Play : Node2D
    {
        private ImageTexture _backgroundTexture;
        private global::Map _map;

        [Export(PropertyHint.Range, "0,10,1")] private int _mapIndex;

        [Export] private string _mapName;
        private MapObject _mapObject;
        private int _pathIndex;
        private AudioStream _stream;
        public bool Finished;

        public string MapFile;
        public float Time;

        public Path CurrentPath => _mapObject.Paths[_pathIndex];
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
            _map = new global::Map(_mapName);
            _map.ReadMap();
            _mapObject = _map.GetMap(_mapIndex);
            _mapObject.BuildPaths();
            GetNode<InputProcessor>("Player/InputProcessor").FeedNotes(_mapObject.Notes);
        }

        public override void _Process(float delta)
        {
            UpdateTime();
            UpdateCurrentPath();
            UpdateHUD();
        }

        private void UpdateHUD()
        {
            var processor = GetNode<InputProcessor>("Player/InputProcessor");
            GetNode<Label>("HUD/Accuracy").Text =
                $"{Math.Round(processor.Counter.GetPercentageAccuracy() * 100, 2)}%";
            GetNode<Label>("HUD/Combo").Text = $"{processor.Counter.Combo}";
            GetNode<Label>("HUD/Score").Text = $"{(int) processor.Counter.Score}";
        }

        private void UpdateCurrentPath()
        {
            if (Finished) return;
            while (CurrentPath.EndTime < Time)
            {
                var player = GetNode<Player>("Player/Player");
                player.RectPosition = CurrentPath.EndPosition;
                if (CurrentPath.TargetNote != null)
                {
                    var tween = player.GetNode<Tween>("Tween");
                    tween.InterpolateProperty(player, "rect_rotation", player.RectRotation,
                        Mathf.Rad2Deg(GetNoteRotation(CurrentPath.TargetNote.Direction)), 0.2f / CurrentPath.Factor);
                    tween.Start();
                }

                _pathIndex++;
                if (_pathIndex >= _mapObject.Paths.Count)
                {
                    Finished = true;
                    return;
                }

                GD.Print(CurrentPath);
            }
        }

        private void UpdateTime()
        {
            if (Finished) return;
            // From https://docs.godotengine.org/en/stable/tutorials/audio/sync_with_audio.html
            Time = (float) (GetNode<AudioStreamPlayer>("AudioStreamPlayer").GetPlaybackPosition() +
                AudioServer.GetTimeSinceLastMix() - AudioServer.GetOutputLatency()) * 1000;
        }

        private void PlaceNotesInScene()
        {
            var noteSpriteScene = GD.Load<PackedScene>("res://Scenes/Note.tscn");
            var pathScene = GD.Load<PackedScene>("res://Scenes/Path.tscn");
            // var mapContainer = GetNode<CanvasLayer>("Map");
            var zInd = _mapObject.Paths.Count;
            foreach (var path in _mapObject.Paths)
            {
                if (path.TargetNote != null)
                {
                    if (noteSpriteScene.Instance() is Node2D noteSprite)
                    {
                        noteSprite.GlobalPosition = path.EndPosition;
                        var targetNoteDirection = (DirectionObject) path.TargetNote.Direction;
                        noteSprite.Rotation = GetNoteRotation(targetNoteDirection);
                        noteSprite.GetNode<Node2D>("Side").Visible = targetNoteDirection.HasSide();
                        noteSprite.ZIndex = zInd--;
                        GetNode("Notes").AddChild(noteSprite);
                    }
                }
                // else
                // {
                //     // Remove or otherwise. This is only used for debugging.
                //     if (pathScene.Instance() is Node2D pathSprite)
                //     {
                //         pathSprite.GlobalPosition = path.EndPosition;
                //         pathSprite.ZIndex = z_ind--;
                //         GetNode("Notes").AddChild(pathSprite);
                //     }
                // }

                GD.Print(path);
            }
        }

        public static float GetNoteRotation(DirectionObject targetNoteDirection)
        {
            return Zero.AngleToPoint(targetNoteDirection.NetDirection) - Mathf.Pi / 2;
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
            var bg = (TextureRect) GetNode(new NodePath("ParallaxBackground/ParallaxLayer/Background"));
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
}