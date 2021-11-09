using Godot;

namespace Quadrecep.UI
{
    public abstract class FileDropHandler : Node
    {
        public override void _Ready()
        {
            base._Ready();
            GetTree().Connect("files_dropped", this, "OnFileDrop");
        }

        public abstract void OnFileDrop(string[] files, int screen);
    }
}