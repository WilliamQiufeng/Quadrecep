using System;
using System.Collections.Generic;
using Godot;
using YamlDotNet.Serialization;

namespace Quadrecep.Gameplay
{
    public class ValueTransformation
    {
        public enum DefaultTypes
        {
            X,
            Y,
            Rotation,
            Alpha
        }

        public int Type = (int)DefaultTypes.Y; // 0-4: x, y, rotation, alpha
        public int Lane = -1;
        public float StartValue, EndValue;
        public float StartTime, EndTime;
        public int TimeTransformationFunction;
        public List<float> TransformationFunctionArgs = new();
        [YamlIgnore] public static List<Func<float, List<float>, float>> RegisteredTransformationFunctions = new();

        public ValueTransformation()
        {
        }

        public ValueTransformation(int type, int lane, float startValue, float endValue, float startTime, float endTime,
            int timeTransformationFunction = 0, List<float> transformationFunctionArgs = default)
        {
            Type = type;
            Lane = lane;
            StartValue = startValue;
            EndValue = endValue;
            StartTime = startTime;
            EndTime = endTime;
            TimeTransformationFunction = timeTransformationFunction;
            TransformationFunctionArgs = transformationFunctionArgs;
        }

        public float TransformWeight(float weight)
        {
            return RegisteredTransformationFunctions[TimeTransformationFunction](weight, TransformationFunctionArgs);
        }

        public float GetWeight(float time)
        {
            return (time - StartTime) / (EndTime - StartTime);
        }

        public float Interpolate(float time)
        {
            return Mathf.Lerp(StartValue, EndValue, TransformWeight(GetWeight(time)));
        }
    }
}