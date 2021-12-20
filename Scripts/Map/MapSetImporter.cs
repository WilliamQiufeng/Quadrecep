using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Quaver.API.Maps;

namespace Quadrecep.Map
{
    public class MapSetImporter
    {
        public static void ImportMapSet(string file)
        {
            if (Global.GetFileExtension(file) == ".qms") ImportQms(file);
            if (Global.GetFileExtension(file) == ".qp") ImportQp(file);
        }

        private static void ImportQms(string file)
        {
            ZipFile.ExtractToDirectory(file,
                Global.RelativeToMap(Path.GetFileNameWithoutExtension(file), absolutePath: true));
        }

        private static void ImportQp(string file)
        {
            var targetDirectory = Global.RelativeToMap(Path.GetFileNameWithoutExtension(file), absolutePath: true);
            ZipFile.ExtractToDirectory(file, targetDirectory);
            var files = (from quaFile in Directory.GetFiles(targetDirectory).Where(x => x.EndsWith(".qua"))
                from gameMode in MapImporter.SupportedGameModes
                select ImportQua(Qua.Parse(quaFile), targetDirectory, gameMode)).ToList();
            var globQua = Qua.Parse(Directory.GetFiles(targetDirectory).First(x => x.EndsWith(".qua")));
            var mapSetObject = GenerateMapSetObject(globQua, files);
            Global.SaveMap(mapSetObject, $"{targetDirectory}/MapSet.qbm");
        }

        /// <summary>
        /// Imports qua file
        /// </summary>
        /// <param name="qua">Qua source</param>
        /// <param name="dir">output dir</param>
        /// <param name="gameMode">output game mode</param>
        /// <returns>file path of result from conversion</returns>
        private static string ImportQua(Qua qua, string dir, string gameMode)
        {
            var importer = MapImporter.GetImporter(gameMode);
            importer.ConvertFrom(qua);
            // Replace extension
            var outputFile = qua.DifficultyName + Global.GetExtensionOfGameMode(gameMode);
            importer.WriteTo($"{dir}/{outputFile}");
            return outputFile;
        }

        public static MapSetObject GenerateMapSetObject(Qua globQua, List<string> mapFiles)
        {
            var resMapSet = new MapSetObject
            {
                Name = globQua.Title,
                Artist = globQua.Artist,
                Creator = globQua.Creator,
                Description = globQua.Description,
                AudioPath = globQua.AudioFile,
                BackgroundPath = globQua.BackgroundFile,
                PreviewTime = globQua.SongPreviewTime,
                Maps = mapFiles,
            };
            return resMapSet;
        }
    }
}