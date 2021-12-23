using Godot;
using Quadrecep.Gameplay;

namespace Quadrecep.GameMode
{
    public abstract class AInputRetriever<T> : Node where T : IClearableInput
    {
        public APlay<T> APlayParent { get; set; }

        /// <summary>
        /// Number of keys to retrieve inputs
        /// </summary>
        public virtual int Keys { get; set; }

        /// <summary>
        /// Input map action name
        /// </summary>
        public virtual string InputName => "";

        public override void _Input(InputEvent @event)
        {
            if (!APlayParent.IsPlaying) return;
            for (var i = 0; i < Keys; i++)
                EnqueueInputs(APlayParent.InputProcessor, APlayParent.DynamicTime + APlayParent.GlobalOffset, @event, $"play_{InputName}_{i}", i);
        }

        /// <summary>
        /// Checks a specific action press/release for a key at a given time for an event<br/>
        /// Calls OnInput when the action is just pressed/released
        /// </summary>
        /// <param name="processor">The processor to communicate with</param>
        /// <param name="time">The time when input</param>
        /// <param name="event">The event caught</param>
        /// <param name="actionName">The action to be checked</param>
        /// <param name="key">The key to update</param>
        protected virtual void EnqueueInputs(AInputProcessor<T> processor, float time, InputEvent @event,
            string actionName, int key)
        {
            if (!InputMap.HasAction(actionName)) return;
            if (@event.IsActionPressed(actionName))
            {
                processor.Inputs[key].Enqueue(new InputEvent<T>(time, key, false));
                OnInput(time, key, false);
            }

            if (@event.IsActionReleased(actionName))
            {
                processor.Inputs[key].Enqueue(new InputEvent<T>(time, key, true));
                OnInput(time, key, true);
            }
        }

        /// <summary>
        /// To be called when a key is just pressed/released
        /// </summary>
        /// <param name="time">Time when the key is pressed/released</param>
        /// <param name="key">The key input</param>
        /// <param name="release">If it is just released or pressed</param>
        protected virtual void OnInput(float time, int key, bool release)
        {
        }
    }
}