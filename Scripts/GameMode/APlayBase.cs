using System;
using System.Threading.Tasks;
using Godot;
using Quadrecep.UI;

namespace Quadrecep.GameMode
{
    public abstract class APlayBase : Node2D
    {
        /// <summary>
        ///     If the gameplay finished
        /// </summary>
        public bool Finished;


        public float GlobalOffset, GlobalVisualOffset;

        /// <summary>
        ///     MapSet path
        /// </summary>
        [Export] public string MapSetFile;

        /// <summary>
        ///     If the gameplay is paused
        /// </summary>
        public bool Paused;

        /// <summary>
        ///     If pitch shifts on rate change
        /// </summary>
        public bool PitchStretch;

        /// <summary>
        ///     Rate of audio player playing
        /// </summary>
        public float Rate = 1.0f;

        /// <summary>
        ///     Time of audio in current frame of _process
        /// </summary>
        public float Time;

        /// <summary>
        ///     ZIndex used for note stack ordering
        /// </summary>
        protected int ZInd;

        /// <summary>
        ///     interval before audio is played
        /// </summary>
        public int PreAudioCountdown => Config.PreAudioCountdown;

        /// <summary>
        ///     From https://docs.godotengine.org/en/stable/tutorials/audio/sync_with_audio.html<br />
        ///     Real-time audio progress
        /// </summary>
        public virtual float DynamicTime => AudioStreamPlayer.Playing && !AudioStreamPlayer.StreamPaused
            ? AudioTime
            : AudioStreamPlayer.GetPlaybackPosition();

        public virtual float AudioTime => (float) (AudioStreamPlayer.GetPlaybackPosition() +
            AudioServer.GetTimeSinceLastMix() - AudioServer.GetOutputLatency()) * 1000;

        /// <summary>
        ///     Path to background node
        /// </summary>
        protected virtual string BackgroundNodePath => "ParallaxBackground/ParallaxLayer/Background";

        /// <summary>
        ///     Background node
        /// </summary>
        public TextureRect Background => GetNode<TextureRect>(BackgroundNodePath);

        /// <summary>
        ///     Path to audioStreamPlayer node
        /// </summary>
        protected virtual string AudioStreamPlayerPath => "AudioStreamPlayer";

        /// <summary>
        ///     audioStreamPlayer node
        /// </summary>
        public AudioStreamPlayer AudioStreamPlayer;

        /// <summary>
        ///     Path to input processor node
        /// </summary>
        protected virtual string InputProcessorPath => "Player/InputProcessor";

        /// <summary>
        ///     Path to input retriever
        /// </summary>
        protected virtual string InputRetrieverPath => "InputRetriever";

        protected virtual string HUDPath => "HUD";

        protected HUD HUD => GetNode<HUD>(HUDPath);

        /// <summary>
        ///     Path to background file
        /// </summary>
        protected virtual string BackgroundPath => "";

        /// <summary>
        ///     Path to audio file
        /// </summary>
        protected virtual string AudioPath => "";

        /// <summary>
        ///     If the gameplay is going on
        /// </summary>
        public bool IsPlaying => !Finished && !Paused;

        public override void _Ready()
        {
            AudioStreamPlayer = GetNode<AudioStreamPlayer>(AudioStreamPlayerPath);
            SetParents();
            LoadMap();
            LoadBackground();
            FeedNotes();
            AfterReady();
        }

        /// <summary>
        ///     Bind parents of the members to *this*
        /// </summary>
        protected virtual void SetParents()
        {
            HUD.PausePanel.Parent = this;
        }

        /// <summary>
        ///     Things to run after _Ready() is called
        /// </summary>
        protected virtual void AfterReady()
        {
            GlobalOffset = Config.KeysAudioOffset;
            GlobalVisualOffset = Config.KeysVisualOffset;
            GD.Print($"Audio Offset: {GlobalOffset} ms");
            GD.Print($"Visual Offset: {GlobalVisualOffset} px");
            Task.Run(LoadAudio);
        }

        /// <summary>
        ///     Loads the map to be played
        /// </summary>
        protected virtual void LoadMap()
        {
            GD.Print($"Loading {MapSetFile}");
            ReadMap();
        }

        /// <summary>
        ///     Reads the map to be played
        /// </summary>
        protected virtual void ReadMap()
        {
        }

        /// <summary>
        ///     Feed notes to input processor
        /// </summary>
        protected virtual void FeedNotes()
        {
        }

        public override void _Process(float delta)
        {
            UpdateTime();
        }

        /// <summary>
        ///     Updates HUD information
        /// </summary>
        public virtual void UpdateHUD()
        {
        }

        /// <summary>
        ///     Updates Time.<br />
        ///     It is called every _Process call, setting Time to DynamicTime
        /// </summary>
        protected virtual void UpdateTime()
        {
            if (!IsPlaying) return;
            var curTime = DynamicTime;
            if (Time < curTime) Time = curTime;
        }

        public override void _Input(InputEvent @event)
        {
            if (!@event.IsActionPressed("play_pause")) return;
            TogglePause();
        }

        internal void TogglePause()
        {
            Paused = !Paused;
            AudioStreamPlayer.StreamPaused = Paused;
            if (Paused) HUD.PausePanel.Popup_();
            else HUD.PausePanel.Hide();
        }

        internal virtual void Retry()
        {
            throw new NotImplementedException();
        }

        internal void PassValuesFrom(APlayBase play)
        {
            MapSetFile = play.MapSetFile;
            Rate = play.Rate;
        }

        internal void GotoMenu()
        {
            Global.SwitchScene(this, Global.SongSelect);
            Global.SongSelect.Slider.UpdateElementFocus();
        }

        /// <summary>
        ///     Loads audio and plays them.<br />
        ///     Delays interval before audio playing.
        /// </summary>
        protected virtual async Task LoadAudio()
        {
            PitchStretch = Config.PitchStretch;
            AudioStreamPlayer.Stream = Global.LoadAudio(Global.RelativeToMap(MapSetFile, AudioPath));
            Global.UpdateRate(AudioStreamPlayer, Rate, PitchStretch);
            await Task.Delay(Mathf.Abs(PreAudioCountdown));
            AudioStreamPlayer.Play();
        }


        /// <summary>
        ///     Loads background file and sets texture
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