using Quadrecep.Map;

namespace Quadrecep.GameMode.Navigate.Map
{
    public class NoteObject : ANoteObject
    {
        public NoteObject(float startTime = default, float length = default, int lane = default) : base(startTime, length, lane)
        {
        }

        public NoteObject()
        {
        }
    }
}