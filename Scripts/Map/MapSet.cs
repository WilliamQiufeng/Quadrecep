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

        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        // Called when the node enters the scene tree for the first time.
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
            SaveMap();
            return true;
        }

        public T GetMap(string mapFile)
        {
            return MapHandler.GetMapHandler(MapFile, mapFile).GetMap<T>();
        }

        public T GetMap(int index)
        {
            return GetMap(MapSetObject.Maps[index]);
        }

        public void ReadMap()
        {
            MapSetObject = Global.DeserializeFromFile<MapSetObject>(MapFile, "MapSet.qbm");
        }

        public void SaveMap()
        {
            Global.SaveMap(MapSetObject, MapFile);
        }

        public override void _Ready()
        {
            GD.Print("Hi");
        }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
    }
}