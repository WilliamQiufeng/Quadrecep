using System.Collections.Generic;

namespace Quadrecep.Map
{
    public class MapObject
    {
        public string DifficultyName { get; set; } = "None";
        public float StartTime { get; set; } = 0;
        public List<NoteObject> Notes { get; set; } = new List<NoteObject>();
        public List<ScrollVelocity> ScrollVelocities { get; set; } = new List<ScrollVelocity>();
        
        public void AddNote(NoteObject note)
        {
            Notes.Add(note);
        }

        public override string ToString()
        {
            return
                $"{nameof(DifficultyName)}: {DifficultyName}, {nameof(StartTime)}: {StartTime}, {nameof(Notes)}: {Notes}";
        }
    }
}