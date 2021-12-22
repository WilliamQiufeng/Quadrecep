using System.Threading;
using System.Threading.Tasks;
using Godot;
using Quadrecep.GameMode;
using Quadrecep.Map;

namespace Quadrecep.UI
{
    public class SongSelectElement : Control
    {
        public const float FocusDuration = 0.2f;
        private static readonly Color UnfocusedColor = new(1, 1, 1, 0.75F);
        private static readonly Color FocusedColor = new(1, 1, 1, 0);

        public static PackedScene Scene;
        private int _difficultyIndex;
        private MapHandler[] _maps;
        private MapSetObject _mapSet;
        private Task[] _tasks;
        public CancellationTokenSource CancellationTokenSource = new();
        public int Index;
        public string MapFile;
        public SongSelectSlider Parent;
        private AudioStreamPlayer AudioStreamPlayer => GetNode<AudioStreamPlayer>("Player");

        private int DifficultyIndex
        {
            get => _difficultyIndex;
            set
            {
                _difficultyIndex = value;
                GetNode<Label>("Difficulty").Text = IsDifficultyLoaded(value)
                    ? _maps[DifficultyIndex].DifficultyName
                    : Global.GetFileName(_mapSet.Maps[DifficultyIndex]);
                GetNode<Label>("GameMode").Text = IsDifficultyLoaded(value)
                    ? _maps[DifficultyIndex].GameModeShortName
                    : $"{Global.GetGameMode(_mapSet.Maps[DifficultyIndex])} (loading)";
            }
        }

        private int Count => _mapSet.Maps.Count;

        public override void _Ready()
        {
            _mapSet = Global.DeserializeFromFile<MapSetObject>(MapFile, "MapSet.qbm");
            GetNode<TextureRect>("Preview").Texture =
                Global.LoadImage(Global.RelativeToMap(MapFile, _mapSet.BackgroundPath));
            GetNode<Label>("Name").Text = _mapSet.Name;
            GetNode<Label>("Author").Text = _mapSet.Creator;
            GetNode<Label>("Artist").Text = _mapSet.Artist;
            AudioStreamPlayer.Stream =
                APlayBase.LoadAudio(Global.RelativeToMap(MapFile, _mapSet.AudioPath));
            DifficultyIndex = 0;
            // GrabFocus();
        }

        private void UpdateRate()
        {
            SetRate(Parent.Rate);
        }

        private bool IsDifficultyLoaded(int index)
        {
            return _maps?[index] != null;
        }

        /// <summary>
        /// Loads maps of a map set simultaneously
        /// </summary>
        public async Task LoadMaps()
        {
            _maps = new MapHandler[Count];
            _tasks = new Task[Count];
            for (var i = 0; i < Count; i++)
            {
                var index = i;
                _tasks[i] = Task.Run(() => _maps[index] = MapHandler.GetMapHandler(MapFile, _mapSet.Maps[index]),
                    CancellationTokenSource.Token);
            }

            await Task.WhenAll(_tasks);
#if DEBUG
            GD.Print($"Done loading {MapFile}");
#endif
            DifficultyIndex = DifficultyIndex; // Refresh state
        }

        public override void _Process(float delta)
        {
            if (!HasFocus()) return;
            if (Input.IsActionJustPressed("ui_up"))
                DifficultyIndex = DifficultyIndex - 1 < 0 ? Count - 1 : DifficultyIndex - 1;
            if (Input.IsActionJustPressed("ui_down")) DifficultyIndex = (DifficultyIndex + 1) % Count;
            if (Input.IsActionJustPressed("ui_play")) PlayMap();
            if (Input.IsActionJustPressed("audio_rate_up")) RateUp();
            if (Input.IsActionJustPressed("audio_rate_down")) RateDown();
        }

        public void SetRate(float rate)
        {
            Parent.Rate = rate;
            // Use Parent.Rate because it clamps the value
            Global.UpdateRate(AudioStreamPlayer, Parent.Rate, Config.PitchStretch);
        }

        public void RateUp() => SetRate(Parent.Rate + 0.05f);
        public void RateDown() => SetRate(Parent.Rate - 0.05f);

        public void _OnPlayPressed()
        {
            if (!HasFocus()) return;
            PlayMap();
        }

        /// <summary>
        /// Loads the game mode of selected map if the map is loaded
        /// </summary>
        private void PlayMap()
        {
            if (!IsDifficultyLoaded(DifficultyIndex))
            {
                GD.Print("Difficulty load not done yet!");
                return;
            }

            CancellationTokenSource.Cancel();
            AudioStreamPlayer.Stop();
            var scene = (APlayBase)_maps[DifficultyIndex].InitScene();
            scene.Rate = Parent.Rate;
            
            GetTree().Root.AddChild(scene);
            GetParent().GetParent().GetParent().GetParent().QueueFree();
        }

        public void _OnFocusEnter()
        {
            GetParent().GetParent().GetParent<SongSelectSlider>().MapIndex = Index;
            var tween = GetNode<Tween>("Mask/Tween");
            tween.InterpolateProperty(tween.GetParent(), "modulate", UnfocusedColor, FocusedColor, FocusDuration,
                Tween.TransitionType.Linear, Tween.EaseType.In);
            tween.Start();
            FadeIn();
            UpdateRate();
        }

        public void _OnFocusExit()
        {
            var tween = GetNode<Tween>("Mask/Tween");
            tween.InterpolateProperty(tween.GetParent(), "modulate", FocusedColor, UnfocusedColor, FocusDuration,
                Tween.TransitionType.Linear, Tween.EaseType.In);
            tween.Start();
            FadeOut();
        }

        private void FadeOut()
        {
            var tween = GetNode<Tween>("Player/Tween");
            tween.InterpolateProperty(tween.GetParent(), "volume_db", 0, -80, FocusDuration);
            tween.Start();
        }

        private void FadeIn()
        {
            AudioStreamPlayer.Playing = true;
            AudioStreamPlayer.Seek(_mapSet.PreviewTime / 1000);
            var tween = GetNode<Tween>("Player/Tween");
            tween.InterpolateProperty(tween.GetParent(), "volume_db", -6, 0, FocusDuration);
            tween.Start();
        }

        public void _OnTweenComplete(Object obj, NodePath key)
        {
            if (!HasFocus()) AudioStreamPlayer.Stop();
        }
    }
}