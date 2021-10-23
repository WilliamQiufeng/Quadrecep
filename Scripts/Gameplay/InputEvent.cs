using Quadrecep.Map;

namespace Quadrecep.Gameplay
{
    public struct InputEvent
    {
        public float Time;
        public int Key;
        public bool Release;
        public bool CountAsInput;
        public readonly NoteObject Note;

        public InputEvent(float time, int key, bool release, bool countAsInput = true, NoteObject note = null)
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
        public bool Matches(InputEvent input)
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