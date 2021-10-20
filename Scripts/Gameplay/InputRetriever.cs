using Godot;

namespace Quadrecep.Gameplay
{
    public class InputRetriever : Node
    {
        public override void _Process(float delta)
        {
            var processor = GetNode<InputProcessor>("../Player/InputProcessor");
            var time = GetNode<Play>("..").Time;
            EnqueueInputs(ref processor, ref time, "play_left", 0);
            EnqueueInputs(ref processor, ref time, "play_down", 1);
            EnqueueInputs(ref processor, ref time, "play_up", 2);
            EnqueueInputs(ref processor, ref time, "play_right", 3);
        }

        private static void EnqueueInputs(ref InputProcessor processor, ref float time, string actionName, int key)
        {
            if (Input.IsActionJustPressed(actionName)) processor.Inputs[key].Enqueue(new InputEvent(time, key, false));
            if (Input.IsActionJustReleased(actionName)) processor.Inputs[key].Enqueue(new InputEvent(time, key, true));
        }
    }
}