using Godot;

namespace Quadrecep.UI
{
    public class SongSelectSlider : Control
    {
        public readonly Directory ContainingDirectory = new();
        public PackedScene Play;

        public override void _Ready()
        {
            Play = ResourceLoader.Load<PackedScene>("res://Scenes/Play.tscn");
            ContainingDirectory.Open($"user://{Map.Map.MapDirectory}");
            LoadElements(ContainingDirectory);
        }

        public void RefreshElements()
        {
            ClearChildren();
            LoadElements(ContainingDirectory);
        }

        private void ClearChildren()
        {
            foreach (Node child in GetNode<HBoxContainer>("ScrollContainer/HBoxContainer").GetChildren())
            {
                child.QueueFree();
            }
        }

        private void LoadElements(Directory dir)
        {
            dir.ListDirBegin();
            var fileName = dir.GetNext();
            while (!string.IsNullOrEmpty(fileName))
            {
                if (dir.CurrentIsDir() && !fileName.StartsWith("."))
                {
                    var element = SongSelectElement.Scene.Instance<SongSelectElement>();
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