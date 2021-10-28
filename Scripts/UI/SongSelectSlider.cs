using Godot;

namespace Quadrecep.UI
{
    public class SongSelectSlider : Control
    {
        public readonly string ContainingDirectory = $"user://{Map.Map.MapDirectory}";
        public PackedScene Play;
        public override void _Ready()
        {
            Play = ResourceLoader.Load<PackedScene>("res://Scenes/Play.tscn");
            var selectElement = ResourceLoader.Load<PackedScene>("res://Scenes/SongSelectElement.tscn");
            var dir = new Directory();
            dir.Open(ContainingDirectory);
            dir.ListDirBegin();
            var fileName = dir.GetNext();
            while (!string.IsNullOrEmpty(fileName))
            {
                if (dir.CurrentIsDir() && !fileName.StartsWith("."))
                {
                    var element = selectElement.Instance<SongSelectElement>();
                    GD.Print(fileName);
                    element.MapFile = fileName;
                    element.PlayScene = Play;
                    GetNode<HBoxContainer>("ScrollContainer/HBoxContainer").AddChild(element);
                }
                fileName = dir.GetNext();
            }
        }
    }
}