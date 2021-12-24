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

        public PausePanel Parent;

        [Export(PropertyHint.Enum, "Resume,Retry,Menu")]
        public ButtonType Type = ButtonType.Resume;

        public override void _Ready()
        {
        }

        private void OnButtonPressed()
        {
            switch (Type)
            {
                case ButtonType.Resume:
                    Parent.Parent.TogglePause();
                    break;
                case ButtonType.Retry:
                    Parent.Parent.Retry();
                    break;
                case ButtonType.Menu:
                    Parent.Parent.GotoMenu();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}