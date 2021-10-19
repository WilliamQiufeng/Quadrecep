using Godot;

public class FPS : Label
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var fps = ((int) Engine.GetFramesPerSecond()).ToString().PadLeft(4);
        Text = $"{fps} FPS";
    }
}