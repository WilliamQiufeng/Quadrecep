using Godot;
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
        private MapSetObject _map;
        public int Index;
        public string MapFile;
        public PackedScene PlayScene;

        public int DifficultyIndex
        {
            get => _difficultyIndex;
            set
            {
                _difficultyIndex = value;
                GetNode<Label>("Difficulty").Text = _map.Maps[DifficultyIndex].DifficultyName;
            }
        }

        public int Count => _map.Maps.Count;

        public override void _Ready()
        {
            _map = Global.ReadMap(MapFile);
            GetNode<TextureRect>("Preview").Texture =
                Global.LoadImage(Global.RelativeToMap(MapFile, _map.BackgroundPath));
            GetNode<Label>("Name").Text = _map.Name;
            GetNode<Label>("Author").Text = _map.Creator;
            GetNode<Label>("Artist").Text = _map.Artist;
            DifficultyIndex = 0;
            GetNode<AudioStreamPlayer>("Player").Stream =
                APlay.LoadAudio(Global.RelativeToMap(MapFile, _map.AudioPath));
            // GrabFocus();
        }

        public override void _Process(float delta)
        {
            if (!HasFocus()) return;
            if (Input.IsActionJustPressed("ui_up"))
                DifficultyIndex = DifficultyIndex - 1 < 0 ? Count - 1 : DifficultyIndex - 1;
            if (Input.IsActionJustPressed("ui_down")) DifficultyIndex = (DifficultyIndex + 1) % Count;
            if (Input.IsActionPressed("ui_play")) PlayMap();
        }

        public void _OnPlayPressed()
        {
            if (!HasFocus()) return;
            PlayMap();
        }

        private void PlayMap()
        {
            GetNode<AudioStreamPlayer>("Player").Stop();
            var play = PlayScene.Instance<APlay>();
            play.MapFile = MapFile;
            play.MapIndex = DifficultyIndex;
            GetTree().Root.AddChild(play);
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
            GetNode<AudioStreamPlayer>("Player").Playing = true;
            GetNode<AudioStreamPlayer>("Player").Seek(_map.PreviewTime / 1000);
            var tween = GetNode<Tween>("Player/Tween");
            tween.InterpolateProperty(tween.GetParent(), "volume_db", -6, 0, FocusDuration);
            tween.Start();
        }

        public void _OnTweenComplete(Object obj, NodePath key)
        {
            if (!HasFocus()) GetNode<AudioStreamPlayer>("Player").Stop();
        }
    }
}