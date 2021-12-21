namespace Quadrecep.Map
{
    /// <summary>
    /// Timing point (Time signature and tempo) changes
    /// </summary>
    public class TimingPoint
    {
        public int Signature;
        public float Tempo;
        public float Time;

        public TimingPoint(float time, float tempo, int signature)
        {
            Time = time;
            Tempo = tempo;
            Signature = signature;
        }

        public TimingPoint()
        {
        }
    }
}