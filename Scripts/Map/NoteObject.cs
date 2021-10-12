using Godot;
using YamlDotNet.Serialization;

namespace Quadrecep.Map
{
    public class NoteObject
    {
    
        public float StartTime { get; set; }
        public float Length { get; set; }
        public int Direction { get; set; }

        public NoteObject(float startTime = default, float length = default, int direction = default)
        {
            StartTime = startTime;
            Length = length;
            Direction = direction;
        }

        public NoteObject()
        {
        }

        public override string ToString()
        {
            return $"{nameof(StartTime)}: {StartTime}, {nameof(Length)}: {Length}, {nameof(Direction)}: {Direction}";
        }
    }
}