using System.Collections.Generic;
using Godot;
using Quadrecep.Database;
using Quadrecep.Map;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Map : Node
{
    [Export(PropertyHint.File, "*.qbm")] public string MapFile { get; set; } = "Test";

    public MapSetObject MapSet;
    
    

    public Map()
    {
    }

    public Map(string mapFile = default)
    {
        MapFile = mapFile;
    }

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
                    DifficultyName = "Default",
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

    public void ReadMap()
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();
        var read = new File();
        read.Open($"user://{MapFile}/MapSet.qbm", File.ModeFlags.Read);
        MapSet = deserializer.Deserialize<MapSetObject>(read.GetAsText());
    }

    public void SaveMap()
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();
        var yaml = serializer.Serialize(MapSet);
        var dir = new Directory();
        dir.MakeDir($"user://{MapFile}");
        var save = new File();
        save.Open($"user://{MapFile}/MapSet.qbm", File.ModeFlags.Write);
        save.StoreString(yaml);
        save.Close();
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