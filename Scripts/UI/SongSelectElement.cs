using System.Collections.Generic;
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
        private MapSetObject _mapSet;
        private List<MapHandler> _maps = new();
        public int Index;
        public string MapFile;
        public PackedScene PlayScene;

        public int DifficultyIndex
        {
            get => _difficultyIndex;
            set
            {
                _difficultyIndex = value;
                GetNode<Label>("Difficulty").Text = _maps[DifficultyIndex].DifficultyName;
                GetNode<Label>("Difficulty/GameMode").Text = _maps[DifficultyIndex].GameModeShortName;
            }
        }

        public int Count => _mapSet.Maps.Count;

        public override void _Ready()
        {
            _mapSet = Global.DeserializeFromFile<MapSetObject>(MapFile, "MapSet.qbm");
            GetNode<TextureRect>("Preview").Texture =
                Global.LoadImage(Global.RelativeToMap(MapFile, _mapSet.BackgroundPath));
            GetNode<Label>("Name").Text = _mapSet.Name;
            GetNode<Label>("Author").Text = _mapSet.Creator;
            GetNode<Label>("Artist").Text = _mapSet.Artist;
            GetNode<AudioStreamPlayer>("Player").Stream =
                APlay.LoadAudio(Global.RelativeToMap(MapFile, _mapSet.AudioPath));
            for (var i = 0; i < Count; i++)
            {
                _maps.Add(MapHandler.GetMapHandler(MapFile, _mapSet.Maps[i]));
            }
            DifficultyIndex = 0;
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
            GetTree().Root.AddChild(_maps[DifficultyIndex].InitScene());
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
            GetNode<AudioStreamPlayer>("Player").Seek(_mapSet.PreviewTime / 1000);
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