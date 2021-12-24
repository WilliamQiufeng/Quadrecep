using System.Collections.Generic;
using System.Linq;
using Godot;
using Quadrecep.GameMode;

namespace Quadrecep.UI
{
    public class PausePanel : Popup
    {
        public APlayBase Parent;

        private List<PausePanelButton> Buttons => GetNode<Control>("CenterContainer/Control").GetChildren()
            .OfType<PausePanelButton>().ToList();

        public override void _Ready()
        {
            foreach (var btn in Buttons) btn.Parent = this;
        }
    }
}