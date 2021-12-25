using System;
using Godot;

namespace Quadrecep.GameMode
{
    public abstract class APlay<T> : APlayBase where T : IClearableInput
    {
        /// <summary>
        ///     InputProcessor node
        /// </summary>
        public AInputProcessor<T> InputProcessor => GetNode<AInputProcessor<T>>(InputProcessorPath);

        /// <summary>
        ///     Input retriever node
        /// </summary>
        public AInputRetriever<T> InputRetriever => GetNode<AInputRetriever<T>>(InputRetrieverPath);


        /// <summary>
        ///     Bind parents of the members to *this*
        /// </summary>
        protected override void SetParents()
        {
            base.SetParents();
            InputRetriever.APlayParent = InputProcessor.APlayParent = this;
        }

        /// <summary>
        ///     Updates HUD information<br />
        ///     By default accuracy, combo and score are updated.
        /// </summary>
        public override void UpdateHUD()
        {
            HUD.Accuracy.Text =
                $"{Math.Round(InputProcessor.Counter.GetPercentageAccuracy() * 100, 2):00.00}%";
            HUD.Combo.Text = $"{InputProcessor.Counter.Combo}";
            HUD.Score.Text = $"{(int) InputProcessor.Counter.Score,8:0000000}";
        }
    }
}