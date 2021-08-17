using Godot;
using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class NoteObject
{
    public float StartTime { get; set; } = 0;
    public float Length { get; set; } = 0;
    public int Direction { get; set; } = 0;
}

public class MapObject
{
    public string DifficultyName { get; set; } = "None";
    public float StartTime { get; set; } = 0;
    public List<NoteObject> Notes { get; set; } = new List<NoteObject>();
}

public class MapSetObject
{
    public float PreviewTime { get; set; } = 0;
    public string Name { get; set; } = "";
    public string Artist { get; set; } = "";
    public string Creator { get; set; } = "";
    public string Description { get; set; } = "";
    public List<MapObject> Maps {get; set; } = new List<MapObject>();
}
public class Map : Node
{
    [Export(PropertyHint.File, "*.qbm")]
    private string map_file;

    private MapSetObject map_set;

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public void CreateMap(string file, string name)
    {
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

    public void SaveMap()
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();
        var yaml = serializer.Serialize(map_set);
        var dir = new Directory();
        dir.MakeDir($"user://{map_set.Name}");
        var save = new File();
        save.Open($"user://{map_set.Name}/map_set.qbm", File.ModeFlags.Write);
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
