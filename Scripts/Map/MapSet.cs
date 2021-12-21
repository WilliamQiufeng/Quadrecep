using System.Collections.Generic;
using Godot;
using Quadrecep.Database;

namespace Quadrecep.Map
{
    public class AMapSet<T> : Node where T : class
    {
        public const string MapDirectory = "Maps";
        public MapSetObject MapSetObject;

        public AMapSet()
        {
        }

        public AMapSet(string mapFile = default)
        {
            MapFile = mapFile;
        }

        [Export(PropertyHint.File, "*.qbm")] public string MapFile { get; set; } = "Test";

        /// <summary>
        /// Creates a map set and add it to database record
        /// </summary>
        /// <param name="name">Name of the map set</param>
        /// <param name="force">allow map set overwrite</param>
        /// <returns>If the creation succeeds</returns>
        public bool CreateMap(string name, bool force = false)
        {
            MapFile = name;
            MapSetObject = new MapSetObject
            {
                Name = name,
                Maps = new List<string>(new[]
                {
                    "Default"
                })
            };
            var record = new MapRecord
            {
                Name = MapSetObject.Name,
                Artist = MapSetObject.Artist,
                AudioPath = MapSetObject.AudioPath,
                BackgroundPath = MapSetObject.BackgroundPath,
                Creator = MapSetObject.Creator
            };
            if (DatabaseHandler.Connection.Table<MapRecord>().Count(x => x.Name == record.Name) != 0 && !force)
                return false;
            DatabaseHandler.Connection.Insert(record);
            MapSetObject.LocalId = record.Id;
            SaveMapSet();
            return true;
        }

        /// <summary>
        /// Gets a map from given <paramref name="mapFile"/>
        /// </summary>
        /// <param name="mapFile">Map file to retrieve maps</param>
        /// <returns>Map read</returns>
        public T GetMap(string mapFile)
        {
            return MapHandler.GetMapHandler(MapFile, mapFile).GetMap<T>();
        }

        /// <summary>
        /// Gets the <paramref name="index"/>'th map of the map set
        /// </summary>
        /// <param name="index">Index of the map</param>
        /// <returns>Map read</returns>
        public T GetMap(int index)
        {
            return GetMap(MapSetObject.Maps[index]);
        }

        /// <summary>
        /// Reads the map set file (.qbm)
        /// </summary>
        public void ReadMapSet()
        {
            MapSetObject = Global.DeserializeFromFile<MapSetObject>(MapFile, "MapSet.qbm");
        }

        /// <summary>
        /// Saves map set file
        /// </summary>
        public void SaveMapSet()
        {
            Global.SerializeToFile(MapSetObject, Global.RelativeToMap(MapFile, "MapSet.qbm"));
        }
    }
}