using System;
using Godot;

namespace Quadrecep.UI
{
    public class PausePanelButton : Button
    {
        public enum ButtonType
        {
            Resume,
            Retry,
            Menu
        }

        public override void _Ready()
        {
            
        }

        [Export(PropertyHint.Enum, "Resume,Retry,Menu")] public ButtonType Type = ButtonType.Resume;
        public PausePanel Parent;
        private void OnButtonPressed()
        {
            switch (Type)
            {
                case ButtonType.Resume:
                    Parent.Parent.TogglePause();
                    break;
                case ButtonType.Retry:
                    break;
                case ButtonType.Menu:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}