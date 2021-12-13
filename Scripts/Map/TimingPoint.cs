namespace Quadrecep.Map
{
    public class TimingPoint
    {
        public float Time;
        public float Tempo;
        public int Signature;

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