using System.Collections.Generic;
using Quadrecep.Map;

namespace Quadrecep.GameMode.Keys.Map
{
    public class MapObject
    {
        public int LaneCount = 4;
        public string DifficultyName { get; set; } = "None";
        public float StartTime { get; set; }
        public List<NoteObject> Notes { get; set; } = new();
        public List<ScrollVelocity> ScrollVelocities { get; set; } = new();
        public List<TimingPoint> TimingPoints { get; set; } = new();


        public override string ToString()
        {
            return
                $"[Map: {nameof(DifficultyName)}: {DifficultyName}, {nameof(StartTime)}: {StartTime}, {nameof(Notes)}: {Notes}, {nameof(ScrollVelocities)}: {ScrollVelocities}]";
        }
    }
}