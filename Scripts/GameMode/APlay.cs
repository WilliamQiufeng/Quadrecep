using System;
using System.Threading.Tasks;
using Godot;

namespace Quadrecep.GameMode
{
    public abstract class APlay<T> : Node2D where T : IClearableInput
    {
        /// <summary>
        /// If the gameplay finished
        /// </summary>
        public bool Finished;

        /// <summary>
        /// MapSet path
        /// </summary>
        [Export] public string MapSetFile;

        /// <summary>
        /// Time of audio in current frame of _process
        /// </summary>
        public float Time;

        /// <summary>
        /// ZIndex used for note stack ordering
        /// </summary>
        protected int ZInd;

        /// <summary>
        /// interval before audio is played
        /// </summary>
        public int PreAudioCountdown => Config.PreAudioCountdown;

        /// <summary>
        /// Real-time audio progress
        /// </summary>
        public virtual float DynamicTime => AudioStreamPlayer.Playing ?
            (float) (AudioStreamPlayer.GetPlaybackPosition() +
                AudioServer.GetTimeSinceLastMix() - AudioServer.GetOutputLatency()) * 1000 : 0f;

        /// <summary>
        /// Path to background node
        /// </summary>
        protected virtual string BackgroundNodePath => "ParallaxBackground/ParallaxLayer/Background";

        /// <summary>
        /// Background node
        /// </summary>
        public TextureRect Background => GetNode<TextureRect>(BackgroundNodePath);
        /// <summary>
        /// Path to audioStreamPlayer node
        /// </summary>
        protected virtual string AudioStreamPlayerPath => "AudioStreamPlayer";

        /// <summary>
        /// audioStreamPlayer node
        /// </summary>
        public AudioStreamPlayer AudioStreamPlayer => GetNode<AudioStreamPlayer>(AudioStreamPlayerPath);
        /// <summary>
        /// Path to input processor node
        /// </summary>
        protected virtual string InputProcessorPath => "Player/InputProcessor";

        /// <summary>
        /// InputProcessor node
        /// </summary>
        public AInputProcessor<T> InputProcessor => GetNode<AInputProcessor<T>>(InputProcessorPath);

        /// <summary>
        /// Path to input retriever
        /// </summary>
        protected virtual string InputRetrieverPath => "InputRetriever";
        /// <summary>
        /// Input retriever node
        /// </summary>
        public AInputRetriever<T> InputRetriever => GetNode<AInputRetriever<T>>(InputRetrieverPath);

        /// <summary>
        /// Path to background file
        /// </summary>
        protected virtual string BackgroundPath => "";
        /// <summary>
        /// Path to audio file
        /// </summary>
        protected virtual string AudioPath => "";
        
        public override void _Ready()
        {
            SetParents();
            LoadMap();
            LoadBackground();
            FeedNotes();
            AfterReady();
        }

        /// <summary>
        /// Bind parents of the members to *this*
        /// </summary>
        protected virtual void SetParents()
        {
            InputRetriever.APlayParent = InputProcessor.APlayParent = this;
        }

        /// <summary>
        /// Things to run after _Ready() is called
        /// </summary>
        protected virtual void AfterReady()
        {
            Task.Run(LoadAudio);
        }

        /// <summary>
        /// Loads the map to be played
        /// </summary>
        protected virtual void LoadMap()
        {
            GD.Print($"Loading {MapSetFile}");
            ReadMap();
        }

        /// <summary>
        /// Reads the map to be played
        /// </summary>
        protected virtual void ReadMap()
        {
        }

        /// <summary>
        /// Feed notes to input processor
        /// </summary>
        protected virtual void FeedNotes()
        {
        }

        public override void _Process(float delta)
        {
            UpdateTime();
            UpdateHUD();
        }

        /// <summary>
        /// Updates HUD information<br/>
        /// By default accuracy, combo and score are updated.
        /// </summary>
        protected virtual void UpdateHUD()
        {
            GetNode<Label>("HUD/Accuracy").Text =
                $"{Math.Round(InputProcessor.Counter.GetPercentageAccuracy() * 100, 2):00.00}%";
            GetNode<Label>("HUD/Combo").Text = $"{InputProcessor.Counter.Combo}";
            GetNode<Label>("HUD/Score").Text = $"{(int) InputProcessor.Counter.Score,8:0000000}";
        }

        /// <summary>
        /// Updates Time.<br/>
        /// It is called every _Process call, setting Time to DynamicTime
        /// </summary>
        protected virtual void UpdateTime()
        {
            if (Finished) return;
            // From https://docs.godotengine.org/en/stable/tutorials/audio/sync_with_audio.html
            Time = DynamicTime;
        }

        /// <summary>
        /// Loads audio and plays them.<br/>
        /// Delays interval before audio playing.
        /// </summary>
        protected virtual async Task LoadAudio()
        {
            AudioStreamPlayer.Stream = LoadAudio(Global.RelativeToMap(MapSetFile, AudioPath));
            await Task.Delay(Mathf.Abs(PreAudioCountdown));
            AudioStreamPlayer.Play();
        }

        /// <summary>
        /// Loads audio file
        /// </summary>
        /// <param name="audioPath">the path to audio file</param>
        /// <returns>audio stream object of the audio</returns>
        /// <exception cref="NotImplementedException">the file format is not supported (not one of mp3, wav or ogg)</exception>
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


        /// <summary>
        /// Loads background file and sets texture
        /// </summary>
        protected virtual void LoadBackground()
        {
            var imgPath = Global.RelativeToMap(MapSetFile, BackgroundPath);
            GD.Print($"Loading background from {imgPath}");
            Background.Texture = Global.LoadImage(imgPath);
            Background.Visible = true;
        }
    }
}