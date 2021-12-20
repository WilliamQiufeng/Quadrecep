using System;
using System.Collections.Generic;
using System.Linq;
using Quadrecep.Map;
using Quaver.API.Maps;
using Quaver.API.Maps.Structures;

namespace Quadrecep.GameMode.Keys.Map
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
            _output = ConvertKeysMap(qua);
        }

        public override bool WriteTo(string path)
        {
            Global.SaveMap(_output, path);
            return true;
        }

        public static MapObject ConvertKeysMap(Qua qua)
        {
            return new MapObject
            {
                LaneCount = qua.Mode == Quaver.API.Enums.GameMode.Keys4 ? 4 : 7,
                DifficultyName = qua.DifficultyName,
                StartTime = qua.TimingPoints[0].StartTime,
                Notes = ConvertNotesKeys(qua.HitObjects),
                ScrollVelocities = qua.SliderVelocities.Select(x => new ScrollVelocity(x.StartTime, x.Multiplier))
                    .ToList(),
                TimingPoints = qua.TimingPoints.Select(x => new TimingPoint(x.StartTime, x.Bpm, (int) x.Signature))
                    .ToList()
            };
        }

        private static List<NoteObject> ConvertNotesKeys(IEnumerable<HitObjectInfo> hitObjects)
        {
            return hitObjects.Select(x =>
                new NoteObject(x.StartTime, x.EndTime == 0 ? 0 : x.EndTime - x.StartTime, x.Lane - 1)).ToList();
        }
    }
}