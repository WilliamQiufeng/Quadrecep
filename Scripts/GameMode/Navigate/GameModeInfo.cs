using Quadrecep.Map;

namespace Quadrecep.GameMode.Navigate
{
    public static class GameModeInfo
    {
        public const string Name = "Navigate";
        public const string Extension = ".qbmn";
        public const string ShortName = "nav";

        public static void Init()
        {
            MapHandler.RegisterHandler(Name, Extension, mapSetPath => new Map.MapHandler(mapSetPath));
        }
    }
}