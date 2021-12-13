using Quadrecep.GameMode;
using Quadrecep.GameMode.Navigate.Map;
using YamlDotNet.Serialization;

namespace Quadrecep.Map
{
    public abstract class ANoteObject
    {
        
        [YamlIgnore] public INoteNode BindNode;

        public ANoteObject(float startTime = default, float length = default, int direction = default)
        {
            StartTime = startTime;
            Length = length;
            Direction = direction;
        }

        public ANoteObject()
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