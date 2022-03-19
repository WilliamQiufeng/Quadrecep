using System;
using System.Collections.Generic;
using Godot;
using YamlDotNet.Serialization;

namespace Quadrecep.Gameplay
{
    public class ValueTransformation
    {
        public int Type = 4; // 0-4: x, y, rotation, alpha
        public float StartValue, EndValue;
        public float StartTime, EndTime;
        public int TimeTransformationFunction;
        public List<float> TransformationFunctionArgs = new();
        [YamlIgnore] public static List<Func<float, List<float>, float>> RegisteredTransformationFunctions = new();

        public ValueTransformation()
        {
        }

        public ValueTransformation(int type, float startValue, float endValue, float startTime, float endTime, int timeTransformationFunction, List<float> transformationFunctionArgs)
        {
            Type = type;
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