using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Quadrecep.Map;
using Quaver.API.Maps;
using Quaver.API.Maps.Structures;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Parser = CommandLine.Parser;

namespace Qua2Qbm
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);
        }

        static void RunOptions(Options opts)
        {
            //handle options
            var qua = Qua.Parse(opts.InputFile);
            var resMap = new MapObject
            {
                DifficultyName = qua.DifficultyName,
                StartTime = qua.TimingPoints[0].StartTime,
                Notes = ConvertNotes(qua.HitObjects)
            };
            var resMapSet = new MapSetObject
            {
                Name = qua.Title,
                Artist = qua.Artist,
                Creator = qua.Creator,
                Description = qua.Description,
                AudioPath = qua.AudioFile,
                BackgroundPath = qua.BackgroundFile,
                PreviewTime = qua.SongPreviewTime,
                Maps = new List<MapObject> {resMap},
            };
            SaveMap(resMapSet, opts.OutputFile);
            Console.WriteLine(qua.Title);
        }

        private static void SaveMap(MapSetObject mapSet, string outputFile)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();
            var streamWriter = new StreamWriter(outputFile);
            serializer.Serialize(streamWriter, mapSet);
            streamWriter.Close();
        }

        private static List<NoteObject> ConvertNotes(List<HitObjectInfo> hitObjects)
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

        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
        }
    }
}