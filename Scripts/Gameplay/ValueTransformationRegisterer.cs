using System.Collections.Generic;
using Godot;

namespace Quadrecep.Gameplay
{
    public class ValueTransformationRegisterer
    {
        public static void RegisterTransformationFunctions()
        {
            ValueTransformation.RegisteredTransformationFunctions.Add(LinearTransform);
            ValueTransformation.RegisteredTransformationFunctions.Add(BezierTransform);
        }

        public static float LinearTransform(float weight, List<float> args) => weight;

        /// <summary>
        /// Cubic Bezier Transformation. <br/>
        /// 0 - args[0] - args[1] - 1 <br/>
        /// Sin ≈ (x: 0.366 - 0.634, y: 0 - 1)<br/>
        /// x^2 ≈ (x: 0.4 - 0.75, y: 0 - 0.5)<br/>
        /// Here's a graph: https://www.geogebra.org/calculator/mkgjpbt7
        /// </summary>
        /// <param name="weight">weight [0..1]</param>
        /// <param name="args">Takes two float arguments for medium weight</param>
        /// <returns>Transformed weight</returns>
        public static float BezierTransform(float weight, List<float> args)
        {
            // var l1 = Mathf.Lerp(0, args[0], weight);
            // var l2 = Mathf.Lerp(args[0], args[1], weight);
            // var l3 = Mathf.Lerp(args[1], 1, weight);
            // var l4 = Mathf.Lerp(l1, l2, weight);
            // var l5 = Mathf.Lerp(l2, l3, weight);
            //
            // return Mathf.Lerp(l4, l5, weight);
            
            // 3aw^3 - 3bw^3 + w^3 - 6aw^2 + 3bw^2 + 3aw
            return (3 * args[0] - 3 * args[1] + 1) * weight * weight * weight +
                   (-6 * args[0] + 3 * args[1]) * weight * weight +
                   args[0] * weight;
        }
        
    }
}