namespace Quadrecep.Gameplay
{
    public struct InputEvent<T>
    {
        public float Time;
        public int Key;
        public bool Release;
        public bool CountAsInput;
        public readonly T Note;

        public InputEvent(float time, int key, bool release, bool countAsInput = true, T note = default)
        {
            Time = time;
            Key = key;
            Release = release;
            CountAsInput = countAsInput;
            Note = note;
        }

        /// <summary>
        ///     Returns if the event matches with another, not considering time
        /// </summary>
        /// <param name="input">Another InputEvent to compare</param>
        /// <returns>if the event matches despite Time</returns>
        public bool Matches(InputEvent<T> input)
        {
            return Key == input.Key && Release == input.Release;
            /*&& CountAsInput == input.CountAsInput*/
        }

        public override string ToString()
        {
            return
                $"{nameof(Time)}: {Time}, {nameof(Key)}: {Key}, {nameof(Release)}: {Release}, {nameof(CountAsInput)}: {CountAsInput}";
        }
    }
}