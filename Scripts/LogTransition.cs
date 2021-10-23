using System;
using Godot;

namespace Quadrecep
{
    public class LogTransition
    {
        private readonly float _length;
        private readonly float _startTime;

        public LogTransition(float startTime, float length)
        {
            _startTime = startTime;
            _length = length;
        }

        public float GetCurrent(float time)
        {
            var val = Math.Log(time - _startTime + 1, _length + 1);
            return double.IsNaN(val) ? 0 : Mathf.Clamp((float) val, 0, 1);
        }
    }
}