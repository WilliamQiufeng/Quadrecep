using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Quadrecep.Gameplay
{
    public class JudgementCounter
    {
        private const float ComboGrouthRate = 1.01f;

        public readonly List<ValidJudgement> ValidJudgements = new List<ValidJudgement>();
        private float _baseMarvelousScore;

        private int _validInputCount;

        public int Combo;
        public int[] JudgementCount = new int[7];

        public JudgementPercentage JudgementPercentage = JudgementPercentage.Default;

        public int MaxScore = 1000000;

        public float Score;

        public int ValidInputCount
        {
            get => _validInputCount;
            set
            {
                _validInputCount = value;
                _baseMarvelousScore = ComboGrouthRate * MaxScore * (ComboGrouthRate - 1) /
                                      (Mathf.Pow(ComboGrouthRate, ValidInputCount + 1) - 1);
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

        public void AddScore(Judgement judgement)
        {
            Score += _baseMarvelousScore * JudgementPercentage.Percentages[(int) judgement] *
                     Mathf.Pow(ComboGrouthRate, Combo - 1);
        }

        public float GetPercentageAccuracy()
        {
            return ValidJudgements.Sum(x => JudgementPercentage.Percentages[(int) x.Judgement]) / ValidJudgements.Count;
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