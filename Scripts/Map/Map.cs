using System.Collections.Generic;
using Godot;
using Quadrecep.Scripts.Database;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class NoteObject
{
    public float StartTime { get; set; }
    public float Length { get; set; }
    public int Direction { get; set; }

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

    public override string ToString()
    {
        return
            $"{nameof(DifficultyName)}: {DifficultyName}, {nameof(StartTime)}: {StartTime}, {nameof(Notes)}: {Notes}";
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
    public int LocalId { get; set; } = -1;
    public int OnlineId { get; set; } = -1;
    public List<MapObject> Maps { get; set; } = new List<MapObject>();

    public override string ToString()
    {
        return
            $"{nameof(PreviewTime)}: {PreviewTime}, {nameof(Name)}: {Name}, {nameof(Artist)}: {Artist}, {nameof(Creator)}: {Creator}, {nameof(Description)}: {Description}, {nameof(Audio)}: {Audio}, {nameof(Background)}: {Background}, {nameof(LocalId)}: {LocalId}, {nameof(OnlineId)}: {OnlineId}, {nameof(Maps)}: {Maps}";
    }
}

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
            Name = MapSet.Name
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