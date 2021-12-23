using System;
using System.Collections.Generic;
using Godot;
using Quadrecep.GameMode;

namespace Quadrecep.Map
{
    /// <summary>
    /// Used to read a map file.<br/>
    /// This is created to resolve maps of different game modes
    /// </summary>
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

        /// <summary>
        /// Creates a <see cref="MapHandler"/> instance for given game mode and reads the given map
        /// </summary>
        /// <param name="mapSetPath">Map set to open</param>
        /// <param name="fileName">Map file to read</param>
        /// <param name="gameMode">Game mode</param>
        /// <returns>a new <see cref="MapHandler"/> for the file</returns>
        public static MapHandler GetMapHandler(string mapSetPath, string fileName, string gameMode)
        {
            var instance = GetNewMapHandler(mapSetPath, gameMode);
            instance.ReadMap(fileName);
            return instance;
        }

        /// <summary>
        /// Creates a <see cref="MapHandler"/> instance for given map.<br/>
        /// The game mode is determined by the extension of <paramref name="fileName"/>
        /// </summary>
        /// <param name="mapSetPath">Map set path</param>
        /// <param name="fileName">Map file</param>
        /// <returns>a new <see cref="MapHandler"/> instance</returns>
        public static MapHandler GetMapHandler(string mapSetPath, string fileName)
        {
            return GetMapHandler(mapSetPath, fileName, Global.GetGameMode(fileName));
        }

        /// <summary>
        /// Creates a <see cref="MapHandler"/> instance for given game mode
        /// </summary>
        /// <param name="mapSetPath">Map set path</param>
        /// <param name="gameMode">Game mode</param>
        /// <returns>a new <see cref="MapHandler"/> instance</returns>
        public static MapHandler GetNewMapHandler(string mapSetPath, string gameMode)
        {
            return Handlers[gameMode](mapSetPath);
        }

        /// <summary>
        /// Registers a <see cref="MapHandler"/> initializer function to a game mode
        /// </summary>
        /// <param name="gameMode">Game mode to bind</param>
        /// <param name="initializer">Initializer of the <see cref="MapHandler"/> for the game mode</param>
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

        public virtual APlayBase InitScene()
        {
            throw new NotImplementedException();
        }
    }
}