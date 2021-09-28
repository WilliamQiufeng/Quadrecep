using Godot;
using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Object = Godot.Object;

public class NoteObject
{
    public float StartTime { get; set; } = 0;
    public float Length { get; set; } = 0;
    public int Direction { get; set; } = 0;

    public NoteObject(float startTime = default, float length = default, int direction = default)
    {
        StartTime = startTime;
        Length = length;
        Direction = direction;
    }

    public NoteObject()
    {
    }

    public override string ToString()
    {
        return $"{nameof(StartTime)}: {StartTime}, {nameof(Length)}: {Length}, {nameof(Direction)}: {Direction}";
    }
}

public class MapObject
{
    public string DifficultyName { get; set; } = "None";
    public float StartTime { get; set; } = 0;
    public List<NoteObject> Notes { get; set; } = new List<NoteObject>();

    public void AddNote(NoteObject note)
    {
        Notes.Add(note);
    }

    public MapObject()
    {
    }

    public override string ToString()
    {
        return $"{nameof(DifficultyName)}: {DifficultyName}, {nameof(StartTime)}: {StartTime}, {nameof(Notes)}: {Notes}";
    }
}

public class MapSetObject
{
    public float PreviewTime { get; set; } = 0;
    public string Name { get; set; } = "";
    public string Artist { get; set; } = "";
    public string Creator { get; set; } = "";
    public string Description { get; set; } = "";
    public string Audio { get; set; } = "audio.mp3";
    public string Background { get; set; } = "background.jpg";
    public int LocalID { get; set; } = -1;
    public int OnlineID { get; set; } = -1;
    public List<MapObject> Maps {get; set; } = new List<MapObject>();

    public MapSetObject()
    {
    }

    public override string ToString()
    {
        return $"{nameof(PreviewTime)}: {PreviewTime}, {nameof(Name)}: {Name}, {nameof(Artist)}: {Artist}, {nameof(Creator)}: {Creator}, {nameof(Description)}: {Description}, {nameof(Maps)}: {Maps}";
    }
}
public class Map : Node
{
    [Export(PropertyHint.File, "*.qbm")] public string map_file { get; set; } = "Test";

    public MapSetObject map_set;

    public Map()
    {
    }

    public Map(string mapFile = default)
    {
        map_file = mapFile;
    }

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public void CreateMap(string file, string name)
    {
        map_file = file;
        map_set = new MapSetObject()
        {
            Name = name,
            Maps = new List<MapObject>(new MapObject[]
            {
                new MapObject()
                {
                    DifficultyName = "Default",
                }
            })
        };
        SaveMap();
    }

    public NoteObject CreateNote(float startTime, float length, int direction)
    {
        return new NoteObject(startTime, length, direction);
    }

    public MapObject GetMap(int index)
    {
        return map_set.Maps[index];
    }

    public void ReadMap()
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();
        var read = new File();
        read.Open($"user://{map_file}/MapSet.qbm", File.ModeFlags.Read);
        map_set = deserializer.Deserialize<MapSetObject>(read.GetAsText());
    }

    public void SaveMap()
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();
        var yaml = serializer.Serialize(map_set);
        var dir = new Directory();
        dir.MakeDir($"user://{map_file}");
        var save = new File();
        save.Open($"user://{map_file}/MapSet.qbm", File.ModeFlags.Write);
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
