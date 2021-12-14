using System;
using Godot;
using Quadrecep.Map;

namespace Quadrecep.GameMode
{
    public abstract class APlay<T> : Node2D where T : IClearableInput
    {
        public bool Finished;

        [Export] public string MapSetFile;

        [Export] public string MapFile;

        public float Time;

        protected int ZInd;

        public float DynamicTime => (float) (AudioStreamPlayer.GetPlaybackPosition() +
            AudioServer.GetTimeSinceLastMix() - AudioServer.GetOutputLatency()) * 1000;

        protected virtual string BackgroundNodePath => "ParallaxBackground/ParallaxLayer/Background";

        public TextureRect Background => GetNode<TextureRect>(BackgroundNodePath);
        protected virtual string AudioStreamPlayerPath => "AudioStreamPlayer";

        public AudioStreamPlayer AudioStreamPlayer => GetNode<AudioStreamPlayer>(AudioStreamPlayerPath);
        protected virtual string InputProcessorPath => "Player/InputProcessor";

        public AInputProcessor<T> InputProcessor => GetNode<AInputProcessor<T>>(InputProcessorPath);

        protected virtual string InputRetrieverPath => "InputRetriever";
        public AInputRetriever<T> InputRetriever => GetNode<AInputRetriever<T>>(InputRetrieverPath);

        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            SetParents();
            LoadMap();
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
            GD.Print($"Loading {MapSetFile}");
            ReadMap();
            FeedNotes();
        }
        protected virtual void ReadMap() {}
        protected virtual void FeedNotes() {}

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
            GetNode<Label>("HUD/Score").Text = $"{(int) InputProcessor.Counter.Score,8:0000000}";
        }

        protected virtual void UpdateTime()
        {
            if (Finished) return;
            // From https://docs.godotengine.org/en/stable/tutorials/audio/sync_with_audio.html
            Time = DynamicTime;
        }

        protected virtual void LoadAudio()
        {
            AudioStreamPlayer.Stream = LoadAudio(Global.RelativeToMap(MapSetFile, AudioPath));
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
            var imgPath = Global.RelativeToMap(MapSetFile, BackgroundPath);
            GD.Print($"Loading background from {imgPath}");
            Background.Texture = Global.LoadImage(imgPath);
            Background.Visible = true;
        }

        protected virtual string BackgroundPath => "";
        protected virtual string AudioPath => "";
    }
}