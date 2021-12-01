using System;

namespace Quadrecep.Gameplay
{
    public class JudgementPercentage
    {
        public static readonly JudgementPercentage Default = new(new[] {1, 0.9825f, 0.8f, 0.7f, -1, 0});

        public readonly float[] Percentages;

        public JudgementPercentage(float[] percentages)
        {
            Percentages = percentages;
        }
    }
}