using System;
using YamlDotNet.Serialization;

namespace Quadrecep.Map
{
    public class ScrollVelocity
    {
        public float Time;
        public float Factor;

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