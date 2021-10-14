namespace Quadrecep.Map
{
    public class ScrollVelocity
    {
        public float Factor;
        public float Time;

        public ScrollVelocity(float time, float factor)
        {
            Time = time;
            Factor = factor;
        }

        public ScrollVelocity()
        {
        }
    }
}