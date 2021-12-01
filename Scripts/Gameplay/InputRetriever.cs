using Godot;

namespace Quadrecep.Gameplay
{
    public class InputRetriever : Node
    {
        public APlay APlayParent { get; set; }

        public override void _Input(Godot.InputEvent @event)
        {
            for (var i = 0; i < 4; i++)
                EnqueueInputs(APlayParent.InputProcessor, APlayParent.Time, @event, $"play_{i}", i);
        }

        private static void EnqueueInputs(AInputProcessor processor, float time, Godot.InputEvent @event,
            string actionName, int key)
        {
            if (@event.IsActionPressed(actionName)) processor.Inputs[key].Enqueue(new InputEvent(time, key, false));
            if (@event.IsActionReleased(actionName)) processor.Inputs[key].Enqueue(new InputEvent(time, key, true));
        }
    }
}