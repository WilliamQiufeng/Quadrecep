using System.Collections.Generic;
using System.Linq;
using Godot;
using Quadrecep.Gameplay;

namespace Quadrecep.GameMode
{
    public abstract class AInputProcessor<T> : Node where T : IClearableInput
    {
        /// <summary>
        /// Judgement Counter to record judgements, sum up scores, give accuracy, etc.
        /// </summary>
        public readonly JudgementCounter Counter = new();

        /// <summary>
        /// The expected inputs to be taken from the player
        /// </summary>
        protected readonly List<Queue<InputEvent<T>>> ExpectedInputs = new();

        /// <summary>
        /// The actual input from a player.<br/>
        /// This can be extended to take inputs from autoplay mod
        /// </summary>
        public readonly List<Queue<InputEvent<T>>> Inputs = new();

        public APlay<T> APlayParent;

        /// <summary>
        /// Judgement set to use.<br/>
        /// This specifies judgement window
        /// </summary>
        public JudgementSet JudgementSet = JudgementSet.Default;

        /// <summary>
        /// Returns the number of tracks of input
        /// </summary>
        public int InputTracks => ExpectedInputs.Count;

        /// <summary>
        /// Returns count of total valid input.<br/>
        /// Sums up all expected inputs where the input counts
        /// </summary>
        public int ValidInputCount => ExpectedInputs.Sum(x => x.Count(inp => inp.CountAsInput));

        public override void _Ready()
        {
            InitTracks(4);
        }

        /// <summary>
        /// Initialise ExpectedInputs and Inputs with <paramref name="trackCount"/> number of tracks
        /// </summary>
        /// <param name="trackCount">The number of tracks to initialise</param>
        protected void InitTracks(int trackCount)
        {
            ExpectedInputs.Clear();
            Inputs.Clear();
            for (var i = 0; i < trackCount; i++)
            {
                ExpectedInputs.Add(new Queue<InputEvent<T>>());
                Inputs.Add(new Queue<InputEvent<T>>());
            }
        }

        /// <summary>
        /// Processes inputs and remove missed notes 60 times per second.<br/>
        /// </summary>
        /// <param name="delta">time passed since last _PhysicsProcess</param>
        public override void _PhysicsProcess(float delta)
        {
            ProcessInputs();
            for (var i = 0; i < InputTracks; i++) RemoveMissed(i, APlayParent.DynamicTime);
        }

        /// <summary>
        ///     Removes events that are too late for the input.
        ///     This prevents the queue being blocked before input events are processed.
        /// </summary>
        /// <param name="input">Input</param>
        private void RemoveMissed(InputEvent<T> input)
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

        /// <summary>
        /// Process inputs from <see cref="Inputs"/>
        /// </summary>
        private void ProcessInputs()
        {
            for (var i = 0; i < InputTracks; i++)
            {
                foreach (var input in Inputs[i]) ProcessInput(input);

                Inputs[i].Clear();
            }
        }

        /// <summary>
        /// Dequeues input event from specified <paramref name="key"/> of <see cref="ExpectedInputs"/><br/>
        /// Calls <see cref="IClearableInput.ClearInput"/> on the dequeued event
        /// </summary>
        /// <param name="key">The key to dequeue</param>
        /// <returns>The event dequeued</returns>
        private InputEvent<T> DequeueLatestInputEvent(int key)
        {
            var latestInputEvent = ExpectedInputs[key].Dequeue();
            latestInputEvent.Note?.ClearInput(key);
            return latestInputEvent;
        }

        /// <summary>
        /// Have a peek at the latest input event from <see cref="ExpectedInputs"/> on the key track
        /// </summary>
        /// <param name="key">Track of <see cref="ExpectedInputs"/> to peek</param>
        /// <returns>peeked event</returns>
        private InputEvent<T> PeekLatestInputEvent(int key)
        {
            return ExpectedInputs[key].Peek();
        }

        /// <summary>
        /// Places visual judgement feedback
        /// </summary>
        /// <param name="input">Input from <see cref="ExpectedInputs"/></param>
        /// <param name="judgement"><see cref="Judgement"/> got from the input</param>
        protected virtual void PlaceJudgementFeedback(InputEvent<T> input, Judgement judgement)
        {
        }

        /// <summary>
        /// Checks queue track of <see cref="ExpectedInputs"/> emptiness
        /// </summary>
        /// <param name="key">Track of <see cref="ExpectedInputs"/></param>
        /// <returns>If the queue is empty</returns>
        private bool IsQueueEmpty(int key)
        {
            return ExpectedInputs[key].Count == 0;
        }

        /// <summary>
        ///     Process a specific input
        /// </summary>
        /// <param name="input"><see cref="InputEvent"/> input</param>
        private void ProcessInput(InputEvent<T> input)
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
        ///     Feeds notes to the <see cref="AInputProcessor{T}"/> to generate <see cref="ExpectedInputs"/>
        /// </summary>
        /// <param name="notes">Notes to generate <see cref="ExpectedInputs"/></param>
        public virtual void FeedNotes(List<T> notes)
        {
        }
    }
}