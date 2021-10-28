using System;
using System.Linq;
using Godot;
using Quadrecep.Gameplay;
using Quadrecep.Map;
using static Godot.Vector2;
using Path = Quadrecep.Map.Path;

namespace Quadrecep
{
    public class Play : Node2D
    {
        private global::Quadrecep.Map.Map _map;
        private readonly PackedScene _noteSpriteScene = GD.Load<PackedScene>("res://Scenes/Note.tscn");
        private int _zInd;
        private int _approachingPathIndex;

        [Export(PropertyHint.Range, "0,10,1")] public int MapIndex;

        [Export] public string MapFile;
        private MapObject _mapObject;
        private int _pathIndex;
        public bool Finished;

        public float Time;

        public Path CurrentPath => _mapObject.Paths[_pathIndex];
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            LoadMap();
            // PlaceNotesInScene();
            GetNode<Label>("HUD/Name").Text = _map.MapSet.Name;
            LoadBackground();
            LoadAudio();
        }

        private void LoadMap()
        {
            _map = new Map.Map(MapFile);
            _map.ReadMap();
            _mapObject = _map.GetMap(MapIndex);
            _mapObject.BuildPaths();
            _zInd = _mapObject.Paths.Count;
            GetNode<InputProcessor>("Player/InputProcessor").FeedNotes(_mapObject.Notes);
        }

        public override void _Process(float delta)
        {
            UpdateTime();
            UpdateCurrentPath();
            UpdateHUD();
            PlaceApproachingNotes();
        }

        private void UpdateHUD()
        {
            var processor = GetNode<InputProcessor>("Player/InputProcessor");

            GetNode<Label>("HUD/Accuracy").Text =
                $"{Math.Round(processor.Counter.GetPercentageAccuracy() * 100, 2):00.00}%";
            GetNode<Label>("HUD/Combo").Text = $"{processor.Counter.Combo}";
            GetNode<Label>("HUD/Score").Text = $"{(int) processor.Counter.Score:0000000}";
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
                        Mathf.Rad2Deg(GetNoteRotation(CurrentPath.TargetNote.Direction)), 0.1f / CurrentPath.Factor);
                    tween.Start();
                }

                _pathIndex++;
                if (_pathIndex < _mapObject.Paths.Count) continue;
                Finished = true;
                return;

                // GD.Print(CurrentPath);
            }
        }

        private void UpdateTime()
        {
            if (Finished) return;
            // From https://docs.godotengine.org/en/stable/tutorials/audio/sync_with_audio.html
            Time = (float) (GetNode<AudioStreamPlayer>("AudioStreamPlayer").GetPlaybackPosition() +
                AudioServer.GetTimeSinceLastMix() - AudioServer.GetOutputLatency()) * 1000;
        }

        [Obsolete("Method deprecated. Use PlaceApproachingNotes instead.")]
        private void PlaceNotesInScene()
        {
            // var pathScene = GD.Load<PackedScene>("res://Scenes/Path.tscn");
            // var mapContainer = GetNode<CanvasLayer>("Map");
            foreach (var path in _mapObject.Paths.Where(path => path.TargetNote != null))
            {
                if (_noteSpriteScene.Instance() is not NoteNode noteSprite) continue;
                noteSprite.Parent = this;
                noteSprite.Note = path.TargetNote;
                noteSprite.GlobalPosition = path.EndPosition;
                var targetNoteDirection = (DirectionObject) path.TargetNote.Direction;
                noteSprite.Rotation = GetNoteRotation(targetNoteDirection);
                noteSprite.GetNode<Node2D>("Side").Visible = targetNoteDirection.HasSide();
                noteSprite.ZIndex = _zInd--;
                GetNode("Notes").AddChild(noteSprite);
            }
        }

        private void PlaceApproachingNotes()
        {
            while (_approachingPathIndex < _mapObject.Paths.Count &&
                   _mapObject.Paths[_approachingPathIndex].StartTime - Time <= NoteNode.FadeInTime)
            {
                if (_noteSpriteScene.Instance() is not NoteNode noteSprite) continue;
                var path = _mapObject.Paths[_approachingPathIndex++];
                if (path.TargetNote == null) continue;
                noteSprite.Parent = this;
                noteSprite.Note = path.TargetNote;
                noteSprite.GlobalPosition = path.EndPosition;
                var targetNoteDirection = (DirectionObject) path.TargetNote.Direction;
                noteSprite.Rotation = GetNoteRotation(targetNoteDirection);
                noteSprite.GetNode<Node2D>("Side").Visible = targetNoteDirection.HasSide();
                noteSprite.ZIndex = _zInd--;
                GetNode("Notes").AddChild(noteSprite);
            }
        }

        public static float GetNoteRotation(DirectionObject targetNoteDirection)
        {
            return Zero.AngleToPoint(targetNoteDirection.NetDirection) - Mathf.Pi / 2;
        }

        private void LoadAudio()
        {
            var audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
            audioPlayer.Stream = LoadAudio(Global.RelativeToMap(MapFile, _map.MapSet.AudioPath));
            audioPlayer.Play();
        }
        
        public static AudioStream LoadAudio(string audioPath)
        {
            var audioFile = new File();
            audioFile.Open(audioPath, File.ModeFlags.Read);
            var buffer = audioFile.GetBuffer((int) audioFile.GetLen());
            if (audioPath.EndsWith(".mp3"))
            {
                var mp3Stream = new AudioStreamMP3();
                mp3Stream.Data = buffer;
                return mp3Stream;
            }

            if (audioPath.EndsWith(".wav"))
            {
                var wavStream = new AudioStreamSample();
                wavStream.Data = buffer;
                return wavStream;
            }

            if (audioPath.EndsWith(".ogg"))
            {
                var oggStream = new AudioStreamOGGVorbis();
                oggStream.Data = buffer;
                return oggStream;
            }
            throw new NotImplementedException();
        }


        private void LoadBackground()
        {
            var imgPath = Global.RelativeToMap(MapFile, _map.MapSet.BackgroundPath);
            GD.Print($"Loading background from {imgPath}");
            var bg = (TextureRect) GetNode(new NodePath("ParallaxBackground/ParallaxLayer/Background"));
            bg.Texture = Global.LoadImage(imgPath);
            bg.Visible = true;
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
    }
}