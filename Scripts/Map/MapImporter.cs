using System;
using System.Collections.Generic;

namespace Quadrecep.Map
{
    /// <summary>
    /// Used to import maps from other games
    /// </summary>
    public abstract class MapImporter
    {
        private static readonly Dictionary<string, Func<MapImporter>> Importers = new();

        public static IEnumerable<string> SupportedGameModes => Importers.Keys;

        /// <summary>
        /// Creates a new <see cref="MapImporter"/> instance for given game mode
        /// </summary>
        /// <param name="gameMode">Game mode of the importer</param>
        /// <returns>a new <see cref="MapImporter"/> instance</returns>
        public static MapImporter GetImporter(string gameMode)
        {
            return Importers[gameMode]();
        }

        /// <summary>
        /// Binds a <see cref="MapImporter"/> initializer function to a given game mode
        /// </summary>
        /// <param name="gameMode">Game mode to bind</param>
        /// <param name="importer">Initializer function</param>
        public static void RegisterImporter(string gameMode, Func<MapImporter> importer)
        {
            Importers.Add(gameMode, importer);
        }

        /// <summary>
        /// Convert <paramref name="source"/> to the map form
        /// </summary>
        /// <param name="source">Source map to convert from</param>
        /// <typeparam name="TType">Type of the source map</typeparam>
        /// <exception cref="NotImplementedException">By default there is no implementation for any conversion from other types of map</exception>
        public virtual void ConvertFrom<TType>(TType source)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write the withholding map to a given <paramref name="path"/>
        /// </summary>
        /// <param name="path">Path to write the map to</param>
        /// <returns>If the writing succeeds</returns>
        /// <exception cref="NotImplementedException">The writing process is not implemented</exception>
        public virtual bool WriteTo(string path)
        {
            throw new NotImplementedException();
        }
    }
}