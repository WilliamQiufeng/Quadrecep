using System.Collections.Generic;
using System.Linq;
using Godot;
using Quadrecep.Gameplay;
using Quadrecep.Map;
using InputEvent = Quadrecep.Gameplay.InputEvent;

namespace Quadrecep.GameMode
{
    public abstract class AInputProcessor : Node
    {
        public readonly JudgementCounter Counter = new();

        protected readonly List<Queue<InputEvent>> ExpectedInputs = new();

        public readonly List<Queue<InputEvent>> Inputs = new();
        public APlay APlayParent;

        public JudgementSet JudgementSet = JudgementSet.Default;
        public int InputTracks => ExpectedInputs.Count;
        protected virtual float Time => APlayParent.Time;

        public int ValidInputCount => ExpectedInputs.Sum(x => x.Count(inp => inp.CountAsInput));

        public override void _Ready()
        {
            InitTracks(4);
        }

        protected void InitTracks(int trackCount)
        {
            ExpectedInputs.Clear();
            Inputs.Clear();
            for (var i = 0; i < trackCount; i++)
            {
                ExpectedInputs.Add(new Queue<InputEvent>());
                Inputs.Add(new Queue<InputEvent>());
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            ProcessInputs();
            for (var i = 0; i < InputTracks; i++) RemoveMissed(i, Time);
        }

        /// <summary>
        ///     Removes events that are too late for the input.
        ///     This prevents the queue being blocked before input events are processed.
        /// </summary>
        /// <param name="input">Input</param>
        private void RemoveMissed(InputEvent input)
        {
            RemoveMissed(input.Key, input.Time);
        }

        /// <summary>
        ///     Removes events that are too late to be input.
        ///     This is called after all input events have been processed.
        ///     This method is made just so that HUD updates misses not until the next input, but every _Process calls.
        /// </summary>
        /// <param name="key">key column to remove</param>
        /// <param name="time">current time</param>
        private void RemoveMissed(int key, float time)
        {
            while (!IsQueueEmpty(key) && JudgementSet.TooLate(PeekLatestInputEvent(key).Time, time))
            {
                var targetInput = DequeueLatestInputEvent(key);
                // if (targetInput.CountAsInput && targetInput.Release) GD.Print("Missed release");
                if (!targetInput.CountAsInput) continue;
                Counter.AddJudgement(Judgement.Miss, JudgementSet.Set.LastOrDefault());
                PlaceJudgementFeedback(targetInput, Judgement.Miss);
                // GD.Print($"\nMiss, {targetInput}\n");
            }
        }

        private void ProcessInputs()
        {
            for (var i = 0; i < InputTracks; i++)
            {
                foreach (var input in Inputs[i]) ProcessInput(input);

                Inputs[i].Clear();
            }
        }

        private InputEvent DequeueLatestInputEvent(int key)
        {
            var latestInputEvent = ExpectedInputs[key].Dequeue();
            if (latestInputEvent.Note?.BindNode != null) latestInputEvent.Note.BindNode.InputLeft[key] = 0;
            return latestInputEvent;
        }

        private InputEvent PeekLatestInputEvent(int key)
        {
            return ExpectedInputs[key].Peek();
        }

        protected virtual void PlaceJudgementFeedback(InputEvent input, Judgement judgement)
        {
        }

        private bool IsQueueEmpty(int key)
        {
            return ExpectedInputs[key].Count == 0;
        }

        /// <summary>
        ///     Process a specific input
        /// </summary>
        /// <param name="input">input input</param>
        private void ProcessInput(InputEvent input)
        {
            if (IsQueueEmpty(input.Key)) return; // Prevent Queue Length=0
            RemoveMissed(input);
            if (IsQueueEmpty(input.Key)) return;
            var targetInput = PeekLatestInputEvent(input.Key);
            if (!input.Matches(targetInput)) return; // Not the same type of event
            if (JudgementSet.NotYet(targetInput.Time, input.Time)) return; // Too soon
            // Dequeue
            DequeueLatestInputEvent(input.Key);
            // At this state we are pretty sure that the input is for targetInput.
            // We take judgement from this and add it to the counter.
            var judgement = JudgementSet.GetJudgement(targetInput.Time, input.Time);
            PlaceJudgementFeedback(targetInput, judgement);
            if (targetInput.CountAsInput) Counter.AddJudgement(judgement, input.Time - targetInput.Time);
        }

        /// <summary>
        ///     Feeds notes to the InputProcessor to generate _expectedInputs.
        /// </summary>
        /// <param name="notes">Notes to generate _expectedInputs</param>
        public virtual void FeedNotes(List<NoteObject> notes)
        {
        }
    }
}