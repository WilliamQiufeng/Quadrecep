using Godot;

namespace Quadrecep.UI
{
    public class HUD : CanvasLayer
    {
        public Label MapSetName => GetNode<Label>("Name");
        public Label Accuracy => GetNode<Label>("Accuracy");
        public Label Score => GetNode<Label>("Score");
        public Label Combo => GetNode<Label>("Combo");
        public PausePanel PausePanel => GetNode<PausePanel>("PausePanel");
        public Fps Fps => GetNode<Fps>("FPS");
    }
}