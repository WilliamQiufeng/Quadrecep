using Godot;

namespace Quadrecep.GameMode
{
    public abstract class AInputRetriever : Node
    {
        public APlay APlayParent { get; set; }

        public virtual int Keys { get; set; }

        public virtual string InputName => "";

        public override void _Input(InputEvent @event)
        {
            for (var i = 0; i < Keys; i++)
                EnqueueInputs(APlayParent.InputProcessor, APlayParent.Time, @event, $"play_{InputName}_{i}", i);
        }

        protected virtual void EnqueueInputs(AInputProcessor processor, float time, InputEvent @event,
            string actionName, int key)
        {
            if (@event.IsActionPressed(actionName))
                processor.Inputs[key].Enqueue(new Gameplay.InputEvent(time, key, false));
            if (@event.IsActionReleased(actionName))
                processor.Inputs[key].Enqueue(new Gameplay.InputEvent(time, key, true));
        }
    }
}