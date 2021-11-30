using Godot;
using Quadrecep.Map;

namespace Quadrecep
{
    public class NoteNode : Node2D
    {
        /// <summary>
        ///     How long before the note is actually pressed
        /// </summary>
        public const float FadeInTime = 1000;

        /// <summary>
        ///     How long the transition from alpha 0 to 1 takes
        /// </summary>
        public const float FadeLength = 750;

        public static PackedScene Scene;

        private bool _finished;

        private LogTransition _transition;
        public DirectionObject InputLeft;
        public NoteObject Note;
        public APlay Parent;

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
            Visible = Modulate.a > 0;
        }

        public void FadeOut()
        {
            Visible = false;
            QueueFree();
        }

        private Color GetAlphaModulate()
        {
            var mod = Modulate;
            mod.a = _transition.GetCurrent(Parent.Time);
            return mod;
        }
    }
}