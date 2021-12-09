using Godot;

namespace Quadrecep
{
    public class Fps : Label
    {
        private int _count = -1;

        public override void _PhysicsProcess(float delta)
        {
            if ((_count = (_count + 1) % 60) != 0) return;
            var fps = ((int) Engine.GetFramesPerSecond()).ToString().PadLeft(4);
            Text = $"{fps} FPS";
        }
    }
}