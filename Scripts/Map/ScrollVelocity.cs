namespace Quadrecep.Map
{
    public class ScrollVelocity
    {
        public float Factor;
        public int Key = -1;
        public float Time;

        public ScrollVelocity(float time, float factor, int key = -1)
        {
            Time = time;
            Factor = factor;
            Key = key;
        }

        public ScrollVelocity()
        {
        }
    }
}