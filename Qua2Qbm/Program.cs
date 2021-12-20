using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using CommandLine;
using Quadrecep.GameMode.Navigate.Map;
using Quadrecep.Map;
using Quaver.API.Enums;
using Quaver.API.Maps;
using Quaver.API.Maps.Structures;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using MapImporter = Quadrecep.GameMode.Keys.Map.MapImporter;
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
            List<string> mapFiles = new();
            Console.WriteLine(globQua.Title);
            var zip = ZipFile.Open($"{opts.OutputFile}/{globQua.Title}.qms", ZipArchiveMode.Create);
            switch (opts.Type)
            {
                case "nav":
                {
                    var maps = Directory.GetFiles(opts.InputFile).Where(file => file.EndsWith(".qua"))
                        .Select(mapFile => Qua.Parse(mapFile)).Where(qua => qua.Mode == GameMode.Keys4)
                        .Select(Quadrecep.GameMode.Navigate.Map.MapImporter.ConvertNavMap).ToList();
                    mapFiles = maps.Select(x => $"{x.DifficultyName}.qbmn").ToList();
                    SaveMaps(zip, maps, mapFiles);
                    break;
                }
                case "keys":
                {
                    var maps = Directory.GetFiles(opts.InputFile).Where(file => file.EndsWith(".qua"))
                        .Select(mapFile => Qua.Parse(mapFile))
                        .Select(MapImporter.ConvertKeysMap).ToList();
                    mapFiles = maps.Select(x => $"{x.DifficultyName}.qbmk").ToList();
                    SaveMaps(zip, maps, mapFiles);
                    break;
                }
            }

            var resMapSet = MapSetImporter.GenerateMapSetObject(globQua, mapFiles);
            var qbmFileWriter = NewFileWriter(zip, "MapSet.qbm");
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

        private static void SaveMaps<T>(ZipArchive zip, List<T> maps, List<string> mapFiles)
        {
            for (var i = 0; i < mapFiles.Count; i++)
            {
                SaveMap(maps[i], NewFileWriter(zip, mapFiles[i]));
            }
        }

        private static StreamWriter NewFileWriter(ZipArchive zip, string fileName)
        {
            var qbmFile = zip.CreateEntry(fileName);
            var qbmFileWriter = new StreamWriter(qbmFile.Open());
            return qbmFileWriter;
        }

        private static void SaveMap<T>(T mapSet, TextWriter outputFile)
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

        static void HandleParseError(IEnumerable<Error> errs)
        {
            //handle errors
        }
    }
}