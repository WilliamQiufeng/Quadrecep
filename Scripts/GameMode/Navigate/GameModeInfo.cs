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
            Global.RegisterExtension(Extension, Name);
            Global.RegisterGameModeShortName(Name, ShortName);
            MapHandler.RegisterHandler(Name, mapSetPath => new Map.MapHandler(mapSetPath));
            MapImporter.RegisterImporter(Name, () => new Map.MapImporter());
        }
    }
}