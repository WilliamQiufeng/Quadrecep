using Godot;
using Quadrecep.Map;

namespace Quadrecep
{
    public class NoteNode : Node2D
    {
        public Play Parent;
        public NoteObject Note;
        public DirectionObject InputLeft;
        private bool _finished = false;

        /// <summary>
        /// How long before the note is actually pressed
        /// </summary>
        public const float FadeInTime = 1000;

        /// <summary>
        /// How long the transition from alpha 0 to 1 takes
        /// </summary>
        public const float FadeLength = 750;

        private LogTransition _transition;
        public override void _Ready()
        {
            _transition = new LogTransition(Note.StartTime - FadeInTime, FadeLength);
            Note.BindNode = this;
            InputLeft = new DirectionObject(Note.Direction);
        }

        public override void _Process(float delta)
        {
            if (!_finished && InputLeft == 0)
            {
                _finished = true;
                FadeOut();
            }
            Modulate = GetAlphaModulate();
        }

        public void FadeOut()
        {
            Visible = false;
        }

        private Color GetAlphaModulate()
        {
            var mod = Modulate;
            mod.a = _transition.GetCurrent(Parent.Time);
            return mod;
        }
    }
}