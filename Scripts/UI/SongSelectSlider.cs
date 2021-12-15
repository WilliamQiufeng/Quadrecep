using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Quadrecep.GameMode.Navigate;
using Quadrecep.Map;

namespace Quadrecep.UI
{
    public class SongSelectSlider : Control
    {
        private int _mapIndex;

        public int MapIndex
        {
            get => _mapIndex;
            set
            {
                if (_mapIndex == value) return;
                _mapIndex = value;
                RefreshFocusChild();
            }
        }

        public SongSelectElement FocusedElement => HBoxContainer.GetChildren()[MapIndex] as SongSelectElement;

        private HBoxContainer HBoxContainer => GetNode<HBoxContainer>("ScrollContainer/HBoxContainer");

        public int ChildrenCount => HBoxContainer.GetChildren().Count;

        private void RefreshFocusChild()
        {
            UpdateElementFocus();
            GetParent<SongSelect>().ChangeBackgroundTexture();
        }

        public override void _Ready()
        {
            Global.MapContainingDirectory.Open($"user://{AMapSet<object>.MapDirectory}");
            LoadElements(Global.MapContainingDirectory);
            RefreshFocusChild();
        }

        public void RefreshElements()
        {
            ClearChildren();
            LoadElements(Global.MapContainingDirectory);
            RefreshFocusChild();
        }

        private void ClearChildren()
        {
            foreach (Node child in HBoxContainer.GetChildren())
            {
                HBoxContainer.RemoveChild(child);
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
                    element.PlayScene = Play.Scene;
                    element.Index = ChildrenCount;
                    HBoxContainer.AddChild(element);
                    Task.Run(() => element.LoadMap());
                }

                fileName = dir.GetNext();
            }
        }

        public override void _Process(float delta)
        {
            // if (!HasFocus()) return;
            if (Input.IsActionJustPressed("ui_left")) MapIndex = MapIndex - 1 < 0 ? ChildrenCount - 1 : MapIndex - 1;
            if (Input.IsActionJustPressed("ui_right")) MapIndex = (MapIndex + 1) % ChildrenCount;
        }

        private void UpdateElementFocus()
        {
            FocusedElement.GrabFocus();
        }
    }
}