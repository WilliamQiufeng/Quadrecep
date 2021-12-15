using System.Collections.Generic;
using Quadrecep.GameMode.Keys;
using Quadrecep.Gameplay;
using YamlDotNet.Serialization;

namespace Quadrecep.GameMode.Keys.Map
{
    public class NoteObject : IClearableInput
    {
        [YamlIgnore] public NoteNode BindNode;

        public List<Path> CustomPaths = new();

        public NoteObject(float startTime = default, float length = default, int lane = default)
        {
            StartTime = startTime;
            Length = length;
            Lane = lane;
        }

        public NoteObject()
        {
        }

        public float StartTime { get; set; }
        public float Length { get; set; }
        public int Lane { get; set; }

        [YamlIgnore] public float EndTime => StartTime + Length;
        [YamlIgnore] public bool IsLongNote => Length > 0;

        public override string ToString()
        {
            return
                $"[Note: {nameof(StartTime)}: {StartTime}, {nameof(Length)}: {Length}, {nameof(Lane)}: {Lane}]";
        }

        public void ClearInput(int key)
        {
            BindNode?.ClearInput();
        }
    }
}