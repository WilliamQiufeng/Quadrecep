using System.Collections.Generic;
using System.Linq;
using Godot;
using Quadrecep.GameMode.Keys.Map;
using Quadrecep.Gameplay;
using InputEvent = Quadrecep.Gameplay.InputEvent<Quadrecep.GameMode.Keys.Map.NoteObject>;

namespace Quadrecep.GameMode.Keys
{
    public class InputProcessor : AInputProcessor<NoteObject>
    {
        private JudgementNode JudgementNode =>
            ((Play) APlayParent).Playfield.GetNode<JudgementNode>("Main/Judgement");
        private int _laneCount = 4;

        public int LaneCount
        {
            get => _laneCount;
            set
            {
                _laneCount = value;
                InitTracks(_laneCount);
            }
        }

        public override void _Ready()
        {
        }

        protected override void PlaceJudgementFeedback(InputEvent input, Judgement judgement)
        {
            if (!input.CountAsInput) return;
            JudgementNode.SetJudgement(judgement);
            JudgementNode.Animate();
        }

        public override void FeedNotes(List<NoteObject> notes)
        {
            GD.Print("Feeding Notes");
            foreach (var note in notes.Where(note => note.Lane < LaneCount))
            {
                // Place note press event
                // Don't clear InputLeft when the note is not a long note and is not primary direction
                ExpectedInputs[note.Lane].Enqueue(new InputEvent(note.StartTime, note.Lane, false,
                    note: note.IsLongNote ? null : note));
                // Places long note releases at primary directions.
                // We assume that there wouldn't be any notes inside a long note.
                // It's the mapper's responsibility not to do so.
                ExpectedInputs[note.Lane].Enqueue(new InputEvent(note.EndTime, note.Lane, true, note.IsLongNote,
                    note.IsLongNote ? note : null));
            }

            Counter.ValidInputCount = ValidInputCount;
            GD.Print("Done Feeding Notes");
            InitJudgementNodePool();
        }

        public virtual void InitJudgementNodePool()
        {
        }
    }
}