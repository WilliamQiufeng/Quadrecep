using Godot;
using Quadrecep.GameMode;

namespace Quadrecep.Gameplay
{
    public abstract class AInputRetriever : Node
    {
        public APlay APlayParent { get; set; }
        public virtual int Keys => 0;
        public virtual string InputName => "";

        public override void _Input(Godot.InputEvent @event)
        {
            for (var i = 0; i < Keys; i++)
                EnqueueInputs(APlayParent.InputProcessor, APlayParent.Time, @event, $"play_{InputName}_{i}", i);
        }

        protected virtual void EnqueueInputs(AInputProcessor processor, float time, Godot.InputEvent @event,
            string actionName, int key)
        {
            if (@event.IsActionPressed(actionName)) processor.Inputs[key].Enqueue(new InputEvent(time, key, false));
            if (@event.IsActionReleased(actionName)) processor.Inputs[key].Enqueue(new InputEvent(time, key, true));
        }
    }
}