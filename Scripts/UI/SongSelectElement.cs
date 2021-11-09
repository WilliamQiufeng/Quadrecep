using Godot;
using Quadrecep.Map;

namespace Quadrecep.UI
{
    public class SongSelectElement : Control
    {
        private const float FocusDuration = 0.2f;
        private static readonly Color UnfocusedColor = new(1, 1, 1, 0.765625F);
        private static readonly Color FocusedColor = new(1, 1, 1, 0);
        private MapSetObject _map;
        public int DifficultyIndex;
        public string MapFile;
        public PackedScene PlayScene;

        public override void _Ready()
        {
            _map = Global.ReadMap(MapFile);
            GetNode<TextureRect>("Preview").Texture =
                Global.LoadImage(Global.RelativeToMap(MapFile, _map.BackgroundPath));
            GetNode<Label>("Name").Text = _map.Name;
            GetNode<Label>("Author").Text = _map.Creator;
            GetNode<Label>("Artist").Text = _map.Artist;
            GetNode<Label>("Difficulty").Text = _map.Maps[DifficultyIndex].DifficultyName;
            GetNode<AudioStreamPlayer>("Player").Stream =
                Play.LoadAudio(Global.RelativeToMap(MapFile, _map.AudioPath));
            // GrabFocus();
        }

        public override void _Process(float delta)
        {
            if (HasFocus() && Input.IsActionPressed("ui_play")) PlayMap();
        }

        public void _OnPlayPressed()
        {
            if (!HasFocus()) return;
            PlayMap();
        }

        private void PlayMap()
        {
            GetNode<AudioStreamPlayer>("Player").Stop();
            var play = PlayScene.Instance<Play>();
            play.MapFile = MapFile;
            GetTree().Root.AddChild(play);
            GetParent().GetParent().GetParent().GetParent().QueueFree();
        }

        public void _OnFocusEnter()
        {
            var tween = GetNode<Tween>("Mask/Tween");
            tween.InterpolateProperty(tween.GetParent(), "modulate", UnfocusedColor, FocusedColor, FocusDuration,
                Tween.TransitionType.Linear, Tween.EaseType.In);
            tween.Start();
        }

        public void _OnFocusExit()
        {
            var tween = GetNode<Tween>("Mask/Tween");
            tween.InterpolateProperty(tween.GetParent(), "modulate", FocusedColor, UnfocusedColor, FocusDuration,
                Tween.TransitionType.Linear, Tween.EaseType.In);
            tween.Start();
        }
    }
}