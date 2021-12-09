using Godot;
using Quadrecep.GameMode.Keys;
using Quadrecep.GameMode.Navigate;
using Quadrecep.Map;
using Quadrecep.UI;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using NoteNode = Quadrecep.GameMode.Navigate.NoteNode;
using Play = Quadrecep.GameMode.Navigate.Play;

namespace Quadrecep
{
    public class Global : Node
    {
        public const string TexturesPath = "res://Textures";

        public static readonly Directory MapContainingDirectory = new();
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            Config.Initialize();
            // DatabaseHandler.Initialize();
            OS.VsyncEnabled = Config.VsyncEnabled;
            OS.WindowFullscreen = Config.WindowFullscreen;
            OS.WindowBorderless = Config.WindowBorderless;
            Play.BaseSV = Config.NavigateScrollSpeed;
            LoadPackedScenes();
            LoadTextures();
        }

        private static void LoadPackedScenes()
        {
            Play.Scene = GD.Load<PackedScene>("res://Scenes/Play.tscn");
            SongSelectElement.Scene = GD.Load<PackedScene>("res://Scenes/SongSelectElement.tscn");
            NoteNode.Scene = GD.Load<PackedScene>("res://Scenes/Note.tscn");
            JudgementNode.Scene = GD.Load<PackedScene>("res://Scenes/Judgement.tscn");
            Receptor.Scene = GD.Load<PackedScene>("res://Scenes/Receptor.tscn");
            GameMode.Keys.NoteNode.Scene = GD.Load<PackedScene>("res://Scenes/KeysNote.tscn");
        }

        private static void LoadTextures()
        {
            JudgementNode.LoadTextures();
            GameMode.Keys.NoteNode.LoadTextures(4);
            GameMode.Keys.NoteNode.LoadTextures(7);
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
        }

        public static Texture LoadImage(string imgPath, string fallback = "")
        {
            if (imgPath.StartsWith("res://"))
            {
                if (!ResourceLoader.Exists(imgPath))
                    return fallback == "" ? null : LoadImage(fallback);
                return ResourceLoader.Load(imgPath) as Texture;
            }

            var img = new Image();
            img.Load(imgPath);
            var texture = new ImageTexture();
            texture.CreateFromImage(img);
            return texture;
        }

        public static string RelativeToMap(string mapFile, string path = "", bool absolutePath = false)
        {
            var root = absolutePath ? OS.GetUserDataDir() : "user://";
            return $"{root}//{Map.Map.MapDirectory}/{mapFile}/{path}";
        }

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

        public static void SwapTexture(TextureRect texture1, TextureRect texture2)
        {
            (texture1.Texture, texture2.Texture) = (texture2.Texture, texture1.Texture);
        }

        private static void SwapModulate(CanvasItem texture1, CanvasItem texture2)
        {
            (texture1.Modulate, texture2.Modulate) = (texture2.Modulate, texture1.Modulate);
        }
    }
}