namespace Quadrecep.Gameplay
{
    public class JudgementPercentage
    {
        public static readonly JudgementPercentage Default = new()
        {
            Percentages = new[] {1, 0.9825f, 0.8f, 0.7f, -1, 0}
        };

        public float[] Percentages = new float[6];
    }
}