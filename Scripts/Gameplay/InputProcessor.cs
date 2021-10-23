using System.Collections.Generic;
using System.Linq;
using Godot;
using Quadrecep.Map;

namespace Quadrecep.Gameplay
{
    public class InputProcessor : Node
    {
        private readonly Queue<InputEvent>[] _expectedInputs =
            {new Queue<InputEvent>(), new Queue<InputEvent>(), new Queue<InputEvent>(), new Queue<InputEvent>()};

        public readonly JudgementCounter Counter = new JudgementCounter();

        public readonly Queue<InputEvent>[] Inputs =
            {new Queue<InputEvent>(), new Queue<InputEvent>(), new Queue<InputEvent>(), new Queue<InputEvent>()};

        public JudgementSet JudgementSet = JudgementSet.Default;

        public int ValidInputCount => _expectedInputs.Sum(x => x.Count(inp => inp.CountAsInput));

        public override void _Ready()
        {
        }

        public override void _Process(float delta)
        {
            var time = GetNode<Play>("../..").Time;
            ProcessInputs();
            for (var i = 0; i < 4; i++) RemoveMissed(i, time);
        }

        /// <summary>
        ///     Removes events that are too late for the input.
        ///     This prevents the queue being blocked before input events are processed.
        /// </summary>
        /// <param name="input">Input</param>
        private void RemoveMissed(InputEvent input)
        {
            // TODO: Support LN Release.
            // Current system supports LN but the player can release and press in the middle without getting miss.
            while (!IsQueueEmpty(input.Key) && JudgementSet.TooLate(PeekLatestInputEvent(input.Key).Time, input.Time))
            {
                var targetInput = DequeueLatestInputEvent(input.Key);
                if (targetInput.CountAsInput) Counter.AddJudgement(Judgement.Miss, JudgementSet.Set.LastOrDefault());
                // GD.Print($"\nMiss, {targetInput}\n");
            }
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
                if (targetInput.CountAsInput) Counter.AddJudgement(Judgement.Miss, JudgementSet.Set.LastOrDefault());
                // GD.Print($"\nMiss, {targetInput}\n");
            }
        }

        private void ProcessInputs()
        {
            for (var i = 0; i < 4; i++)
            {
                foreach (var input in Inputs[i]) ProcessInput(input);

                Inputs[i].Clear();
            }
        }

        private InputEvent DequeueLatestInputEvent(int key)
        {
            var latestInputEvent = _expectedInputs[key].Dequeue();
            if (latestInputEvent.Note != null) latestInputEvent.Note.BindNode.InputLeft[key] = 0;
            return latestInputEvent;
        }

        private InputEvent PeekLatestInputEvent(int key)
        {
            return _expectedInputs[key].Peek();
        }

        private bool IsQueueEmpty(int key)
        {
            return _expectedInputs[key].Count == 0;
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
            if (targetInput.CountAsInput) Counter.AddJudgement(judgement, input.Time - targetInput.Time);
            // GD.Print($"\nNew judgement: {judgement}, {input.Time - targetInput.Time}ms diff\n{targetInput}\n");
        }

        /// <summary>
        ///     Feeds notes to the InputProcessor to generate _expectedInputs.
        /// </summary>
        /// <param name="notes">Notes to generate _expectedInputs</param>
        public void FeedNotes(List<NoteObject> notes)
        {
            foreach (var note in notes)
            {
                DirectionObject dir = note.Direction;
                var primaryDir = dir.GetPrimaryDirection();
                for (var i = 0; i < dir.Direction.Length; i++)
                {
                    if (dir.Direction[i] != 1) continue;
                    // Place note press event
                    _expectedInputs[i].Enqueue(new InputEvent(note.StartTime, i, false));
                    // Place note release event if the note isn't long note.
                    // Since it's not a long note, it shouldn't count as a valid input.
                    if (!note.IsLongNote) _expectedInputs[i].Enqueue(new InputEvent(note.EndTime, i, true, false));
                }
                // GD.Print($"{dir}\n->{primaryDir}");

                if (!note.IsLongNote) continue;
                // Places long note releases at primary directions.
                // We assume that there wouldn't be any notes inside a long note.
                // It's the mapper's responsibility not to do so.
                for (var i = 0; i < 4; i++)
                    if (primaryDir.Direction[i] == 1)
                        _expectedInputs[i].Enqueue(new InputEvent(note.EndTime, i, true));
            }

            Counter.ValidInputCount = ValidInputCount;

            // for (var i = 0; i < 4; i++)
            // {
            //     GD.Print($"Column {i}");
            //     foreach (var input in _expectedInputs[i])
            //     {
            //         GD.Print(input);
            //     }
            // }
        }
    }
}