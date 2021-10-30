using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using CommandLine;
using Quadrecep.Map;
using Quaver.API.Maps;
using Quaver.API.Maps.Structures;
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
            var globQua = Qua.Parse(Directory.GetFiles(opts.InputFile).First(file => file.EndsWith(".qua")));
            var maps = Directory.GetFiles(opts.InputFile).Where(file => file.EndsWith(".qua"))
                .Select(mapFile => Qua.Parse(mapFile)).Select(qua => new MapObject
                {
                    DifficultyName = qua.DifficultyName, StartTime = qua.TimingPoints[0].StartTime,
                    Notes = ConvertNotes(qua.HitObjects)
                }).ToList();
            var resMapSet = new MapSetObject
            {
                Name = globQua.Title,
                Artist = globQua.Artist,
                Creator = globQua.Creator,
                Description = globQua.Description,
                AudioPath = globQua.AudioFile,
                BackgroundPath = globQua.BackgroundFile,
                PreviewTime = globQua.SongPreviewTime,
                Maps = maps,
            };
            Console.WriteLine(globQua.Title);
            var zip = ZipFile.Open(opts.OutputFile, ZipArchiveMode.Create);
            var qbmFile = zip.CreateEntry("MapSet.qbm");
            var qbmFileWriter = new StreamWriter(qbmFile.Open());
            SaveMap(resMapSet, qbmFileWriter);
            foreach (var file in Directory.GetFiles(opts.InputFile).Where(file => !file.EndsWith(".qua")))
            {
                var fileName = new FileInfo(file).Name;
                var entry = zip.CreateEntry(fileName);
                WriteZipEntry(entry, file);
                Console.WriteLine($"{fileName}: {file}");
            }
            zip.Dispose();
        }

        private static void SaveMap(MapSetObject mapSet, TextWriter outputFile)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();
            serializer.Serialize(outputFile, mapSet);
            outputFile.Close();
        }

        private static void WriteZipEntry(ZipArchiveEntry entry, string path)
        {
            var writer = new BinaryWriter(entry.Open());
            var binaryReader = new BinaryReader(File.Open(path, FileMode.Open));
            var buffer = new byte[binaryReader.BaseStream.Length];
            binaryReader.Read(buffer);
            writer.Write(buffer);
            writer.Close();
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