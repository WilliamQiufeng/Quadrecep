using System.Collections.Generic;
using Godot;
using Quadrecep.Gameplay;
using Quadrecep.Map;
using InputEvent = Quadrecep.Gameplay.InputEvent;

namespace Quadrecep.GameMode.Navigate
{
    public class InputProcessor : AInputProcessor
    {
        public override float Time => APlayParent.Time;

        public override void _Ready()
        {
            InitTracks(4);
        }

        protected override void PlaceJudgementFeedback(InputEvent input, Judgement judgement)
        {
            if (input.Note == null) return;
            var judgementNode = JudgementNode.Scene.Instance<JudgementNode>();
            judgementNode.GlobalPosition = input.Note.BindNode.GlobalPosition;
            judgementNode.Judgement = judgement;
            APlayParent.GetNode<CanvasLayer>("JudgementFeedbacks").AddChild(judgementNode);
        }

        public override void FeedNotes(List<NoteObject> notes)
        {
            foreach (var note in notes)
            {
                DirectionObject dir = note.Direction;
                var primaryDir = dir.GetPrimaryDirection();
                for (var i = 0; i < dir.Direction.Length; i++)
                {
                    if (dir.Direction[i] != 1) continue;
                    // Place note press event
                    // Don't clear InputLeft when the note is not a long note and is not primary direction
                    ExpectedInputs[i].Enqueue(new InputEvent(note.StartTime, i, false,
                        note: note.IsLongNote && primaryDir[i] == 1 ? null : note));

                    // Places long note releases at primary directions.
                    // We assume that there wouldn't be any notes inside a long note.
                    // It's the mapper's responsibility not to do so.
                    if (note.IsLongNote && primaryDir.Direction[i] == 1)
                        ExpectedInputs[i].Enqueue(new InputEvent(note.EndTime, i, true, note: note));
                    // Place note release event if the currently processing note direction is not primary..
                    // As a side input it shouldn't be long note..
                    if (primaryDir[i] != 1) ExpectedInputs[i].Enqueue(new InputEvent(note.EndTime, i, true, false));
                }
            }

            Counter.ValidInputCount = ValidInputCount;
        }
    }
}