using System;

namespace Quadrecep.Gameplay
{
    public class JudgementSet
    {
        public static readonly JudgementSet Default = new JudgementSet
        {
            Set = new float[] {20, 30, 50, 70, 90, 110}
        };

        public float[] Set = new float[6];

        public Judgement GetJudgement(float targetTime, float actualTime)
        {
            var diff = Math.Abs(actualTime - targetTime);
            for (var i = 0; i < Set.Length; i++)
                if (diff < Set[i])
                    return (Judgement) i;

            return Judgement.Miss;
        }

        public bool IsMiss(float targetTime, float actualTime)
        {
            return GetJudgement(targetTime, actualTime) == Judgement.Miss;
        }

        /// <summary>
        ///     If the note is out of range
        /// </summary>
        /// <param name="targetTime"></param>
        /// <param name="actualTime"></param>
        /// <returns></returns>
        public bool NotYet(float targetTime, float actualTime)
        {
            return targetTime - actualTime > Set[5];
        }

        public bool TooLate(float targetTime, float actualTime)
        {
            return IsMiss(targetTime, actualTime) && !NotYet(targetTime, actualTime);
        }
    }
}