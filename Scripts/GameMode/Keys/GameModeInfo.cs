using Quadrecep.Map;

namespace Quadrecep.GameMode.Keys
{
    public class GameModeInfo
    {
        public const string Name = "Keys";
        public const string ShortName = "key";
        public const string Extension = ".qbmk";

        public static void Init()
        {
            Global.RegisterExtension(Extension, Name);
            MapHandler.RegisterHandler(Name, mapSetPath => new Map.MapHandler(mapSetPath));
            MapImporter.RegisterImporter(Name, () => new Map.MapImporter());
        }
    }
}