using System.Collections.Generic;
using Godot;
using YamlDotNet.Serialization;

namespace Quadrecep.Map
{
    public class MapObject
    {
        public string DifficultyName { get; set; } = "None";
        public float StartTime { get; set; } = 0;
        public List<NoteObject> Notes { get; set; } = new List<NoteObject>();
        public List<ScrollVelocity> ScrollVelocities { get; set; } = new List<ScrollVelocity>();
        [YamlIgnore] public List<Path> Paths { get; set; } = new List<Path>();

        public void AddNote(NoteObject note)
        {
            Notes.Add(note);
        }

        public void AddSV(ScrollVelocity sv)
        {
            ScrollVelocities.Add(sv);
        }

        /// <summary>
        /// note(t) #     #
        /// sv  (t)   # #   
        /// </summary>
        public void BuildPaths()
        {
            if (Notes.Count == 0) return;
            var svIndex = 0;
            var lastNoteStartTime = 0f;
            var currentPosition = new Vector2(0, 0);
            var lastSVFactor = 1.0f;
            DirectionObject direction = 0b0010; // Up

            foreach (var note in Notes)
            {
                UpdateSVIndex(ref svIndex, lastNoteStartTime);
                var slicedStartTime = lastNoteStartTime;
                // If there are SV changes between [lastNoteStartTime...currentNoteStartTime), 
                // break the path into sections
                for (;
                    svIndex < ScrollVelocities.Count && ScrollVelocities[svIndex].Time < note.StartTime;
                    svIndex++)
                {
                    var path = new Path(lastSVFactor, slicedStartTime, ScrollVelocities[svIndex].Time, direction,
                        currentPosition, null);
                    Paths.Add(path);
                    lastSVFactor = ScrollVelocities[svIndex].Factor;
                    slicedStartTime = ScrollVelocities[svIndex].Time;
                    currentPosition = path.EndPosition;
                }
                // Add final path where the next note is `note`.
                // If there isn't any SVs between, the start time will be lastNoteStartTime
                Paths.Add(new Path(lastSVFactor, slicedStartTime, note.StartTime, direction,
                    currentPosition, note));

                lastNoteStartTime = note.StartTime;
                currentPosition = Paths[Paths.Count - 1].EndPosition;
                direction = note.Direction;
            }
        }

        private void UpdateSVIndex(ref int svIndex, float leastTime)
        {
            while (svIndex < ScrollVelocities.Count && ScrollVelocities[svIndex].Time < leastTime) svIndex++;
        }


        public override string ToString()
        {
            return $"[Map: {nameof(DifficultyName)}: {DifficultyName}, {nameof(StartTime)}: {StartTime}, {nameof(Notes)}: {Notes}, {nameof(ScrollVelocities)}: {ScrollVelocities}, {nameof(Paths)}: {Paths}]";
        }
    }
}