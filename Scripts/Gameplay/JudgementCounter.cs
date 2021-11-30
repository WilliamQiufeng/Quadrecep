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
        public int[] JudgementCount = new int[6];

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
            GD.Print($"New judgement: {judgement}, {timeDiff}ms diff");
        }

        private void AddScore(Judgement judgement)
        {
            Score += _baseMarvelousScore * JudgementPercentage.Percentages[(int) judgement] *
                     Mathf.Pow(ComboGrowthRate, Combo - 1);
        }

        public float GetPercentageAccuracy()
        {
            return Mathf.Clamp(
                ValidJudgements.Sum(x => JudgementPercentage.Percentages[(int) x.Judgement]) / ValidJudgements.Count, 0,
                100);
        }

        public float GetStandardDeviance()
        {
            return (float) Math.Sqrt(ValidJudgements.Sum(x => Mathf.Pow(x.TimeDifference, 2)) /
                                     ValidJudgements.Count);
        }

        public float GetMean()
        {
            return ValidJudgements.Sum(x => x.TimeDifference) / ValidJudgements.Count;
        }
    }
}