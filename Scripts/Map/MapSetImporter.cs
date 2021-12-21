using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Quaver.API.Maps;

namespace Quadrecep.Map
{
    /// <summary>
    /// Used to import a whole map set in a single file, usually compressed.
    /// </summary>
    public class MapSetImporter
    {
        public static void ImportMapSet(string file)
        {
            if (Global.GetFileExtension(file) == ".qms") ImportQms(file);
            if (Global.GetFileExtension(file) == ".qp") ImportQp(file);
        }

        /// <summary>
        /// Imports qms, the condensed map set file natively used by Quadrecep.<br/>
        /// Since it's natively supported, it can be imported simply by extracting the file
        /// </summary>
        /// <param name="file">The file path to the .qbm file</param>
        private static void ImportQms(string file)
        {
            ZipFile.ExtractToDirectory(file,
                Global.RelativeToMap(Path.GetFileNameWithoutExtension(file), absolutePath: true));
        }

        /// <summary>
        /// Imports qp, the condensed map set file natively used by Quaver.<br/>
        /// This method extracts the file into the map set directory,<br/>
        /// and tries to convert every difficulty to into every game modes that registered <see cref="MapImporter"/>.
        /// </summary>
        /// <param name="file">The file path to the .qp file</param>
        private static void ImportQp(string file)
        {
            var targetDirectory = Global.RelativeToMap(Path.GetFileNameWithoutExtension(file), absolutePath: true);
            ZipFile.ExtractToDirectory(file, targetDirectory);
            var files = (from x in Directory.GetFiles(targetDirectory)
                where x.EndsWith(".qua")
                from gameMode in MapImporter.SupportedGameModes
                select ImportQua(Qua.Parse(x), targetDirectory, gameMode)
                into res
                where res != null
                select res).ToList();

            var globQua = Qua.Parse(Directory.GetFiles(targetDirectory).First(x => x.EndsWith(".qua")));
            var mapSetObject = GenerateMapSetObject(globQua, files);
            Global.SerializeToFile(mapSetObject, $"{targetDirectory}/MapSet.qbm");
        }

        /// <summary>
        ///     Converts qua file to a given <paramref name="gameMode"/>.<br/>
        ///     This first finds the corresponding <see cref="MapImporter"/> for given <paramref name="gameMode"/>,<br/>
        ///     then converts the deserialized <see cref="Qua"/> object into corresponding MapObject,<br/>
        ///     then saves the converted MapObject
        /// </summary>
        /// <param name="qua">Qua source</param>
        /// <param name="dir">Output dir</param>
        /// <param name="gameMode">Output game mode</param>
        /// <returns>File path of result from conversion</returns>
        private static string ImportQua(Qua qua, string dir, string gameMode)
        {
            var importer = MapImporter.GetImporter(gameMode);
            importer.ConvertFrom(qua);
            // Replace extension
            var outputFile = qua.DifficultyName + Global.GetExtensionOfGameMode(gameMode);
            var res = importer.WriteTo($"{dir}/{outputFile}");
            return res ? outputFile : null;
        }

        /// <summary>
        /// Generates <see cref="MapSetObject"/> from <paramref name="globQua"/>
        /// </summary>
        /// <param name="globQua">The .qua file to retrieve metadata from</param>
        /// <param name="mapFiles">Map files list</param>
        /// <returns>Generated <see cref="MapSetObject"/></returns>
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
                Maps = mapFiles
            };
            return resMapSet;
        }
    }
}