using System;
using System.Collections.Generic;

namespace Quadrecep.Map
{
    public abstract class MapImporter
    {
        private static readonly Dictionary<string, Func<MapImporter>> Importers = new();

        public static IEnumerable<string> SupportedGameModes => Importers.Keys;

        public static MapImporter GetImporter(string gameMode)
        {
            return Importers[gameMode]();
        }

        public static void RegisterImporter(string gameMode, Func<MapImporter> importer)
        {
            Importers.Add(gameMode, importer);
        }

        public virtual void ConvertFrom<TType>(TType source)
        {
            throw new NotImplementedException();
        }

        public virtual bool WriteTo(string path)
        {
            throw new NotImplementedException();
        }
    }
}