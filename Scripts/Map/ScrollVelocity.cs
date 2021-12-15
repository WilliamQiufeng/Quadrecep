namespace Quadrecep.Map
{
    public class ScrollVelocity
    {
        public float Factor;
        public float Time;
        public int Key = -1;

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