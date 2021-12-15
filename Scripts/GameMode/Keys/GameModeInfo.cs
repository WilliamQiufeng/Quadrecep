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
            MapHandler.RegisterHandler(Name, Extension, mapSetPath => new Map.MapHandler(mapSetPath));
        }
    }
}