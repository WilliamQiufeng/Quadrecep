using System;
using Godot;
using Quadrecep.Gameplay;
using Quadrecep.Map;

namespace Quadrecep.GameMode
{
    public abstract class APlay : Node2D
    {
        public bool Finished;
        protected Map.Map Map;

        [Export] public string MapFile;

        [Export(PropertyHint.Range, "0,10,1")] public int MapIndex;
        protected MapObject MapObject;

        public float Time;

        public float DynamicTime => (float) (AudioStreamPlayer.GetPlaybackPosition() +
            AudioServer.GetTimeSinceLastMix() - AudioServer.GetOutputLatency()) * 1000;

        protected int ZInd;
        protected virtual string BackgroundNodePath => "ParallaxBackground/ParallaxLayer/Background";

        public TextureRect Background => GetNode<TextureRect>(BackgroundNodePath);
        protected virtual string AudioStreamPlayerPath => "AudioStreamPlayer";

        public AudioStreamPlayer AudioStreamPlayer => GetNode<AudioStreamPlayer>(AudioStreamPlayerPath);
        protected virtual string InputProcessorPath => "Player/InputProcessor";

        public AInputProcessor InputProcessor => GetNode<AInputProcessor>(InputProcessorPath);

        protected virtual string InputRetrieverPath => "InputRetriever";
        public AInputRetriever InputRetriever => GetNode<AInputRetriever>(InputRetrieverPath);

        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            SetParents();
            LoadMap();
            GetNode<Label>("HUD/Name").Text = Map.MapSet.Name;
            LoadBackground();
            LoadAudio();
            AfterReady();
        }

        protected virtual void SetParents()
        {
            InputRetriever.APlayParent = InputProcessor.APlayParent = this;
        }

        protected virtual void AfterReady()
        {
        }

        protected virtual void LoadMap()
        {
            GD.Print($"Loading {MapFile}");
            Map = new Map.Map(MapFile);
            Map.ReadMap();
            MapObject = Map.GetMap(MapIndex);
            MapObject.BuildPaths();
            ZInd = MapObject.Paths.Count;
            InputProcessor.FeedNotes(MapObject.Notes);
        }

        public override void _Process(float delta)
        {
            UpdateTime();
            UpdateHUD();
        }

        protected virtual void UpdateHUD()
        {
            GetNode<Label>("HUD/Accuracy").Text =
                $"{Math.Round(InputProcessor.Counter.GetPercentageAccuracy() * 100, 2):00.00}%";
            GetNode<Label>("HUD/Combo").Text = $"{InputProcessor.Counter.Combo}";
            GetNode<Label>("HUD/Score").Text = $"{(int) InputProcessor.Counter.Score, 8:0000000}";
        }

        protected virtual void UpdateTime()
        {
            if (Finished) return;
            // From https://docs.godotengine.org/en/stable/tutorials/audio/sync_with_audio.html
            Time = DynamicTime;
        }

        protected virtual void LoadAudio()
        {
            AudioStreamPlayer.Stream = LoadAudio(Global.RelativeToMap(MapFile, Map.MapSet.AudioPath));
            AudioStreamPlayer.Play();
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


        protected virtual void LoadBackground()
        {
            var imgPath = Global.RelativeToMap(MapFile, Map.MapSet.BackgroundPath);
            GD.Print($"Loading background from {imgPath}");
            Background.Texture = Global.LoadImage(imgPath);
            Background.Visible = true;
        }
    }
}