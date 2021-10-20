namespace Quadrecep.Gameplay
{
    public struct ValidJudgement
    {
        public Judgement Judgement;
        public float TimeDifference;

        public ValidJudgement(Judgement judgement, float timeDifference)
        {
            Judgement = judgement;
            TimeDifference = timeDifference;
        }

        public ValidJudgement UseSet(JudgementSet set)
        {
            return new ValidJudgement(set.GetJudgement(TimeDifference, 0), TimeDifference);
        }
    }
}