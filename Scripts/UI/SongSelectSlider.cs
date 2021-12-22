using System.Threading;
using System.Threading.Tasks;
using Godot;
using Quadrecep.Map;

namespace Quadrecep.UI
{
    public class SongSelectSlider : Control
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private int _mapIndex;
        private float _rate = 1.0f;
        public SongSelect Parent;
        public float Rate { 
            get => _rate;
            set
            {
                _rate = Mathf.Clamp(value, 0.50f, 2.00f);
                if (Parent != null) Parent.Rate.Text = $"{_rate:0.00}x";
            } 
        }

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
            var fileCount = 0;
            while (!string.IsNullOrEmpty(fileName))
            {
                if (dir.CurrentIsDir() && !fileName.StartsWith("."))
                {
                    var element = CreateElement(fileName);
                    HBoxContainer.AddChild(element);
                    Task.Run(() => element.LoadMaps());
                    fileCount++;
                }

                fileName = dir.GetNext();
            }
            GD.Print($"{fileCount} Map sets");

            if (fileCount != 0) return;
            var stubElement = CreateStubElement();
            HBoxContainer.AddChild(stubElement);
            GD.Print("Stub element is created");
        }

        private SongSelectElement CreateStubElement()
        {
            var element = SongSelectElement.Scene.Instance<SongSelectElement>();
            element.Parent = this;
            element.Stub = true;
            return element;
        }

        private SongSelectElement CreateElement(string fileName)
        {
            var element = SongSelectElement.Scene.Instance<SongSelectElement>();
            GD.Print(fileName);
            element.MapFile = fileName;
            element.Index = ChildrenCount;
            element.CancellationTokenSource = _cancellationTokenSource;
            element.Parent = this;
            return element;
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