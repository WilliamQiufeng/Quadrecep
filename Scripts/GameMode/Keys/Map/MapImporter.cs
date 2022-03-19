using System;
using System.Collections.Generic;
using System.Linq;
using Quadrecep.Gameplay;
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
            Global.SerializeToFile(_output, path);
            return true;
        }

        public static MapObject ConvertKeysMap(Qua qua)
        {
            var lastNoteTime = qua.HitObjects.Max(x => x.StartTime + x.EndTime);
            return new MapObject
            {
                LaneCount = qua.Mode == Quaver.API.Enums.GameMode.Keys4 ? 4 : 7,
                DifficultyName = qua.DifficultyName,
                StartTime = qua.TimingPoints[0].StartTime,
                Notes = ConvertNotesKeys(qua.HitObjects),
                ScrollVelocities = qua.SliderVelocities.Select(x => new ScrollVelocity(x.StartTime, x.Multiplier))
                    .ToList(),
                Transformations = ConvertTransformation(qua.SliderVelocities, lastNoteTime),
                TimingPoints = qua.TimingPoints.Select(x => new TimingPoint(x.StartTime, x.Bpm, (int) x.Signature))
                    .ToList()
            };
        }

        public static List<ValueTransformation> ConvertTransformation(IEnumerable<SliderVelocityInfo> svs, float lastNoteTime)
        {
            var res = new List<ValueTransformation>();
            float currentTime = 0;
            float startValue = 0;
            float currentSV = 1;
            foreach (var sv in svs)
            {
                var endValue = startValue + currentSV * (sv.StartTime - currentTime);
                res.Add(new ValueTransformation(1, -1, startValue, endValue, currentTime, sv.StartTime));
                currentTime = sv.StartTime;
                startValue = endValue;
                currentSV = sv.Multiplier;
            }
            res.Add(new ValueTransformation(1, -1, startValue, startValue + currentSV * (lastNoteTime - currentTime), currentTime, lastNoteTime));

            return res;
        }

        private static List<NoteObject> ConvertNotesKeys(IEnumerable<HitObjectInfo> hitObjects)
        {
            return hitObjects.Select(x =>
                new NoteObject(x.StartTime, x.EndTime == 0 ? 0 : x.EndTime - x.StartTime, x.Lane - 1)).ToList();
        }
    }
}