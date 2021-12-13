using Quadrecep.GameMode.Navigate.Map;

namespace Quadrecep.GameMode.Navigate
{
    public static class GameModeInfo
    {
        public const string Name = "Navigate";
        public const string Extension = ".qbmn";
        public const string ShortName = "nav";
        public static void Init()
        {
            Quadrecep.Map.MapHandler.RegisterHandler(Name, Extension, mapSetPath => new MapHandler(mapSetPath));
        }
        
    }
}