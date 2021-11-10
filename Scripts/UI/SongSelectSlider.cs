using Godot;

namespace Quadrecep.UI
{
    public class SongSelectSlider : Control
    {
        public readonly Directory ContainingDirectory = new();
        private int _mapIndex = -1;

        public int MapIndex
        {
            get => _mapIndex;
            set
            {
                if (_mapIndex == value) return;
                _mapIndex = value;
                UpdateElementFocus();
                GetParent<SongSelect>().ChangeBackgroundTexture();
            }
        }

        public SongSelectElement FocusedElement => HBoxContainer.GetChildren()[MapIndex] as SongSelectElement;

        private HBoxContainer HBoxContainer => GetNode<HBoxContainer>("ScrollContainer/HBoxContainer");

        public int Count => HBoxContainer.GetChildren().Count;

        public override void _Ready()
        {
            ContainingDirectory.Open($"user://{Map.Map.MapDirectory}");
            LoadElements(ContainingDirectory);
            MapIndex = 0;
        }

        public void RefreshElements()
        {
            ClearChildren();
            LoadElements(ContainingDirectory);
            MapIndex = 0;
        }

        private void ClearChildren()
        {
            foreach (Node child in HBoxContainer.GetChildren())
                child.QueueFree();
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
                    element.PlayScene = Play.Scene;
                    element.Index = Count;
                    HBoxContainer.AddChild(element);
                }

                fileName = dir.GetNext();
            }
        }

        public override void _Process(float delta)
        {
            // if (!HasFocus()) return;
            if (Input.IsActionJustPressed("ui_left")) MapIndex = MapIndex - 1 < 0 ? Count - 1 : MapIndex - 1;
            if (Input.IsActionJustPressed("ui_right")) MapIndex = (MapIndex + 1) % Count;
        }

        private void UpdateElementFocus()
        {
            FocusedElement.GrabFocus();
        }
    }
}