using System.Collections.Generic;
using Godot;
using Quadrecep.Map;
using YamlDotNet.Serialization;
using Path = Quadrecep.Gameplay.Path;

namespace Quadrecep.GameMode.Keys.Map
{
    public class MapObject
    {
        public string DifficultyName { get; set; } = "None";
        public float StartTime { get; set; }
        public List<NoteObject> Notes { get; set; } = new();
        public List<ScrollVelocity> ScrollVelocities { get; set; } = new();
        public List<TimingPoint> TimingPoints { get; set; } = new();
        [YamlIgnore] public List<Path> Paths { get; set; } = new();

        public void AddNote(NoteObject note)
        {
            Notes.Add(note);
        }

        public void AddSV(ScrollVelocity sv)
        {
            ScrollVelocities.Add(sv);
        }

        /// <summary>
        ///     note(t) #     #
        ///     sv  (t)   # #
        /// </summary>
        public void BuildPaths()
        {
        }

        private void UpdateSVIndex(ref int svIndex, float leastTime)
        {
            while (svIndex < ScrollVelocities.Count && ScrollVelocities[svIndex].Time < leastTime) svIndex++;
        }


        public override string ToString()
        {
            return
                $"[Map: {nameof(DifficultyName)}: {DifficultyName}, {nameof(StartTime)}: {StartTime}, {nameof(Notes)}: {Notes}, {nameof(ScrollVelocities)}: {ScrollVelocities}, {nameof(Paths)}: {Paths}]";
        }
    }
}