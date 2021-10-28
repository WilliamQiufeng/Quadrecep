using Godot;
using Quadrecep.Map;

namespace Quadrecep.UI
{
    public class SongSelectElement : Control
    {
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
        }

        public void _OnPlayPressed()
        {
            GetNode<AudioStreamPlayer>("Player").Stop();
            var play = PlayScene.Instance<Play>();
            play.MapFile = MapFile;
            GetTree().Root.AddChild(play);
            GetParent().GetParent().GetParent().GetParent().QueueFree();
        }
    }
}