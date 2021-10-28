using Godot;
using Quadrecep.Database;
using Quadrecep.Map;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Quadrecep
{
    public class Global : Node
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            DatabaseHandler.Initialize();
            OS.VsyncEnabled = false;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
        }

        public static Texture LoadImage(string imgPath)
        {
            var img = new Image();
            img.Load(imgPath);
            var texture = new ImageTexture();
            texture.CreateFromImage(img);
            return texture;
        }

        public static string RelativeToMap(string mapFile, string path = "") => $"user://{Map.Map.MapDirectory}/{mapFile}/{path}";

        public static MapSetObject ReadMap(string mapFile)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();
            var read = new File();
            read.Open(RelativeToMap(mapFile, "MapSet.qbm"), File.ModeFlags.Read);
            return deserializer.Deserialize<MapSetObject>(read.GetAsText());
        }

        public static void SaveMap(MapSetObject mapSet, string mapFile)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(mapSet);
            var dir = new Directory();
            dir.MakeDir($"user://{mapFile}");
            var save = new File();
            save.Open(RelativeToMap(mapFile, "MapSet.qbm"), File.ModeFlags.Write);
            save.StoreString(yaml);
            save.Close();
        }
    }
}