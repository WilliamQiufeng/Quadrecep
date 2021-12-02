namespace Quadrecep.Gameplay
{
    public class JudgementPercentage
    {
        public static readonly JudgementPercentage Default = new(new[] {1, 0.9825f, 0.65f, 0.25f, -1, -0.5f});

        public readonly float[] Percentages;

        public JudgementPercentage(float[] percentages)
        {
            Percentages = percentages;
        }
    }
}