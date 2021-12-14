using YamlDotNet.Serialization;

namespace Quadrecep.GameMode.Navigate.Map
{
    public class NoteObject : IClearableInput
    {
        [YamlIgnore] public NoteNode BindNode;
        public void ClearInput(int key)
        {
            if (BindNode != null) BindNode.InputLeft[key] = 0;
        }

        public NoteObject(float startTime = default, float length = default, int direction = default)
        {
            StartTime = startTime;
            Length = length;
            Direction = direction;
        }

        public NoteObject()
        {
        }

        public float StartTime { get; set; }
        public float Length { get; set; }
        public int Direction { get; set; }

        [YamlIgnore] public float EndTime => StartTime + Length;
        [YamlIgnore] public bool IsLongNote => Length > 0;

        public override string ToString()
        {
            return
                $"[Note: {nameof(StartTime)}: {StartTime}, {nameof(Length)}: {Length}, {nameof(Direction)}: {new DirectionObject(Direction)}]";
        }
    }
}