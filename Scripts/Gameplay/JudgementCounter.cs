using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Quadrecep.Gameplay
{
    public class JudgementCounter
    {
        private const float ComboGrowthRate = 1.0001f;

        private const int MaxScore = 1000000;

        public readonly List<ValidJudgement> ValidJudgements = new();
        private float _baseMarvelousScore;

        private int _validInputCount;

        public int Combo;
        public int[] JudgementCount = new int[Enum.GetNames(typeof(Judgement)).Length];

        public JudgementPercentage JudgementPercentage = JudgementPercentage.Default;

        public float Score;

        public int ValidInputCount
        {
            get => _validInputCount;
            set
            {
                _validInputCount = value;
                _baseMarvelousScore = MaxScore * (ComboGrowthRate - 1) /
                                      (Mathf.Pow(ComboGrowthRate, ValidInputCount) - 1);
                GD.Print($"Valid Input Count: {ValidInputCount}, Base Marv Score: {_baseMarvelousScore}");
            }
        }

        public void AddJudgement(Judgement judgement, float timeDiff)
        {
            JudgementCount[(int) judgement]++;
            ValidJudgements.Add(new ValidJudgement(judgement, timeDiff));
            if (judgement == Judgement.Miss) Combo = 0;
            else Combo++;
            AddScore(judgement);
            // GD.Print($"New judgement: {judgement}, {timeDiff}ms diff");
        }

        private void AddScore(Judgement judgement)
        {
            Score += _baseMarvelousScore * JudgementPercentage.Percentages[(int) judgement] *
                     Mathf.Pow(ComboGrowthRate, Combo - 1);
        }

        public float GetPercentageAccuracy()
        {
            return ValidJudgements.Count == 0
                ? 0
                : Mathf.Clamp(
                    ValidJudgements.Average(x => JudgementPercentage.Percentages[(int) x.Judgement]), 0,
                    100);
        }

        public float GetStandardDeviance()
        {
            return ValidJudgements.Count == 0
                ? 0
                : (float) Math.Sqrt(ValidJudgements.Average(x => Mathf.Pow(x.TimeDifference, 2)));
        }

        public float GetMean()
        {
            return ValidJudgements.Count == 0 ? 0 : ValidJudgements.Average(x => x.TimeDifference);
        }
    }
}