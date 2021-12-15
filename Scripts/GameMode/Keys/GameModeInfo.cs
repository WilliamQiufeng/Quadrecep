

using Quadrecep.GameMode.Keys.Map;

namespace Quadrecep.GameMode.Keys
{
    public class GameModeInfo
    {
        public const string Name = "Keys";
        public const string ShortName = "key";
        public const string Extension = ".qbmk";

        public static void Init()
        {
            Quadrecep.Map.MapHandler.RegisterHandler(Name, Extension, mapSetPath => new MapHandler(mapSetPath));
        }
    }
}