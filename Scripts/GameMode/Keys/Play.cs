using Godot;

namespace Quadrecep.GameMode.Keys
{
    public class Play : APlay
    {
        protected override string BackgroundNodePath => "Background";
        protected override string InputProcessorPath => "InputProcessor";
        public static PackedScene Scene;

        protected string PlayfieldPath => "Playfield";
        public Playfield Playfield => GetNode<Playfield>(PlayfieldPath);

        protected override void SetParents()
        {
            base.SetParents();
            Playfield.Parent = this;
        }

        protected override void AfterReady()
        {
            base.AfterReady();
            Playfield.InitField();
        }
    }
}