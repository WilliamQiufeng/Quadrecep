using Godot;

namespace Quadrecep
{
    public class Player : Control
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
            var play = GetNode<Play>("../..");
            if (play.Finished) return;
            RectPosition = play.CurrentPath.GetPosition(play.Time);
            // GetNode<Camera2D>("../../Camera2D").Offset = RectPosition;
        }
    }
}