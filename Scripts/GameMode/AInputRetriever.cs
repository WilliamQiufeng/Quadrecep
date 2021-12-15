using Godot;
using Quadrecep.Gameplay;

namespace Quadrecep.GameMode
{
    public abstract class AInputRetriever<T> : Node where T : IClearableInput
    {
        public APlay<T> APlayParent { get; set; }

        public virtual int Keys { get; set; }

        public virtual string InputName => "";

        public override void _Input(InputEvent @event)
        {
            for (var i = 0; i < Keys; i++)
                EnqueueInputs(APlayParent.InputProcessor, APlayParent.Time, @event, $"play_{InputName}_{i}", i);
        }

        protected virtual void EnqueueInputs(AInputProcessor<T> processor, float time, InputEvent @event,
            string actionName, int key)
        {
            if (!InputMap.HasAction(actionName)) return;
            if (@event.IsActionPressed(actionName))
                processor.Inputs[key].Enqueue(new InputEvent<T>(time, key, false));
            if (@event.IsActionReleased(actionName))
                processor.Inputs[key].Enqueue(new InputEvent<T>(time, key, true));
        }
    }
}