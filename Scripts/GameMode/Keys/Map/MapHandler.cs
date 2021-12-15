using Godot;

namespace Quadrecep.GameMode.Keys.Map
{
    public class MapHandler : Quadrecep.Map.MapHandler
    {
        private string _mapFile;
        private MapObject _mapObject;

        public MapHandler(string mapSetPath) : base(mapSetPath)
        {
        }

        public override PackedScene Scene => Play.Scene;
        public override string DifficultyName => _mapObject.DifficultyName;
        public override string GameModeShortName => GameModeInfo.ShortName;

        public override void ReadMap(string file)
        {
            _mapFile = file;
            _mapObject = Global.DeserializeFromFile<MapObject>(MapSetPath, file);
        }

        public override T GetMap<T>() where T : class
        {
            return _mapObject as T;
        }

        public override Node InitScene()
        {
            var scene = Play.Scene.Instance<Play>();
            scene.MapSetFile = MapSetPath;
            scene.MapFile = _mapFile;
            return scene;
        }
    }
}