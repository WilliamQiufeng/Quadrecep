using System;
using System.Collections.Generic;
using System.Linq;
using Quadrecep.Map;
using Quaver.API.Maps;
using Quaver.API.Maps.Structures;

namespace Quadrecep.GameMode.Navigate.Map
{
    public class MapImporter : Quadrecep.Map.MapImporter
    {
        private MapObject _output;

        public override void ConvertFrom<TType>(TType source)
        {
            if (typeof(TType) == typeof(Qua)) ConvertFrom(source as Qua);
            else throw new ArgumentException($"Conversion of type {typeof(TType)}");
        }

        private void ConvertFrom(Qua qua)
        {
            _output = ConvertNavMap(qua);
        }

        public override void WriteTo(string path)
        {
            Global.SaveMap(_output, path);
        }

        public static MapObject ConvertNavMap(Qua qua)
        {
            if (qua.Mode != Quaver.API.Enums.GameMode.Keys4) return null;
            return new MapObject
            {
                DifficultyName = qua.DifficultyName,
                StartTime = qua.TimingPoints[0].StartTime,
                Notes = ConvertNotes(qua.HitObjects),
                ScrollVelocities = qua.SliderVelocities.Select(x => new ScrollVelocity(x.StartTime, x.Multiplier))
                    .ToList(),
                TimingPoints = qua.TimingPoints.Select(x => new TimingPoint(x.StartTime, x.Bpm, (int) x.Signature))
                    .ToList()
            };
        }

        public static List<NoteObject> ConvertNotes(List<HitObjectInfo> hitObjects)
        {
            var notes = new List<NoteObject>();
            var lastTime = 0f;
            var keys = new[] {0, 0, 0, 0};
            var length = 0f;
            foreach (var hitObject in hitObjects)
            {
                if (hitObject.StartTime != lastTime)
                {
                    var note = new NoteObject(lastTime, length, new DirectionObject(keys));
                    notes.Add(note);
                    lastTime = hitObject.StartTime;
                    keys = new[] {0, 0, 0, 0};
                }

                keys[hitObject.Lane - 1] = 1;
                length = hitObject.IsLongNote ? hitObject.EndTime - lastTime : length;
            }

            if (keys != new[] {0, 0, 0, 0})
            {
                var note = new NoteObject(lastTime, length, new DirectionObject(keys));
                notes.Add(note);
            }

            Console.WriteLine($"Final: {notes.LastOrDefault()}");

            return notes;
        }
    }
}