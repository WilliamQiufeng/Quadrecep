namespace Quadrecep.Gameplay
{
    public class JudgementPercentage
    {
        public static readonly JudgementPercentage Default = new JudgementPercentage
        {
            Percentages = new[] {1, 0.9825f, 0.8f, 0.7f, 0.5f, -1, 0}
        };

        public float[] Percentages = new float[7];
    }
}