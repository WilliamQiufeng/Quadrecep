using System.Collections.Generic;
using Godot;
using Quadrecep.Database;

namespace Quadrecep.Map
{
    public class Map : Node
    {
        public MapSetObject MapSet;
        public const string MapDirectory = "Maps";

        public Map()
        {
        }

        public Map(string mapFile = default)
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
            MapSet = new MapSetObject
            {
                Name = name,
                Maps = new List<MapObject>(new[]
                {
                    new MapObject
                    {
                        DifficultyName = "Default"
                    }
                })
            };
            var record = new MapRecord
            {
                Name = MapSet.Name,
                Artist = MapSet.Artist,
                AudioPath = MapSet.AudioPath,
                BackgroundPath = MapSet.BackgroundPath,
                Creator = MapSet.Creator
            };
            if (DatabaseHandler.Connection.Table<MapRecord>().Count(x => x.Name == record.Name) != 0 && !force)
                return false;
            DatabaseHandler.Connection.Insert(record);
            MapSet.LocalId = record.Id;
            SaveMap();
            return true;
        }

        public NoteObject CreateNote(float startTime, float length, int direction)
        {
            return new NoteObject(startTime, length, direction);
        }

        public MapObject GetMap(int index)
        {
            return MapSet.Maps[index];
        }

        public void ReadMap() => MapSet = Global.ReadMap(MapFile);

        public void SaveMap() => Global.SaveMap(MapSet, MapFile);

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