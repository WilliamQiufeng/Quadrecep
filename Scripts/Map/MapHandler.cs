using System;
using System.Collections.Generic;
using Godot;

namespace Quadrecep.Map
{
    public abstract class MapHandler
    {
        private static readonly Dictionary<string, Func<string, MapHandler>> Handlers = new();

        protected readonly string MapSetPath;

        protected MapHandler(string mapSetPath)
        {
            MapSetPath = mapSetPath;
        }

        public virtual PackedScene Scene => throw new NotImplementedException();
        public virtual string DifficultyName => throw new NotImplementedException();
        public virtual string GameModeShortName => throw new NotImplementedException();

        public static MapHandler GetMapHandler(string mapSetPath, string fileName, string gameMode)
        {
            var instance = GetNewMapHandler(mapSetPath, gameMode);
            instance.ReadMap(fileName);
            return instance;
        }

        public static MapHandler GetMapHandler(string mapSetPath, string fileName)
        {
            return GetMapHandler(mapSetPath, fileName, Global.GetGameMode(fileName));
        }

        public static MapHandler GetNewMapHandler(string mapSetPath, string gameMode)
        {
            return Handlers[gameMode](mapSetPath);
        }

        public static void RegisterHandler(string gameMode, Func<string, MapHandler> initializer)
        {
            Handlers.Add(gameMode, initializer);
        }

        public virtual T GetMap<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public virtual void ReadMap(string file)
        {
            throw new NotImplementedException();
        }

        public virtual Node InitScene()
        {
            throw new NotImplementedException();
        }
    }
}