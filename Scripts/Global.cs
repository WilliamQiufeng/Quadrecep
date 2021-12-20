using System.Collections.Generic;
using System.IO;
using Godot;
using Quadrecep.GameMode.Keys;
using Quadrecep.GameMode.Navigate.Map;
using Quadrecep.Map;
using Quadrecep.UI;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Directory = Godot.Directory;
using File = Godot.File;
using GameModeInfo = Quadrecep.GameMode.Navigate.GameModeInfo;
using JudgementNode = Quadrecep.GameMode.Navigate.JudgementNode;
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
            GameModeInfo.Init();
            GameMode.Keys.GameModeInfo.Init();
        }

        private static void LoadPackedScenes()
        {
            Play.Scene = GD.Load<PackedScene>("res://Scenes/Play.tscn");
            GameMode.Keys.Play.Scene = GD.Load<PackedScene>("res://Scenes/PlayKeys.tscn");
            SongSelectElement.Scene = GD.Load<PackedScene>("res://Scenes/SongSelectElement.tscn");
            NoteNode.Scene = GD.Load<PackedScene>("res://Scenes/Note.tscn");
            JudgementNode.Scene = GD.Load<PackedScene>("res://Scenes/Judgement.tscn");
            GameMode.Keys.JudgementNode.Scene = GD.Load<PackedScene>("res://Scenes/KeysJudgement.tscn");
            Receptor.Scene = GD.Load<PackedScene>("res://Scenes/Receptor.tscn");
            GameMode.Keys.NoteNode.Scene = GD.Load<PackedScene>("res://Scenes/KeysNote.tscn");
        }

        private static void LoadTextures()
        {
            JudgementNode.LoadTextures();
            GameMode.Keys.JudgementNode.LoadTextures();
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
            return $"{root}//{MapSet.MapDirectory}/{mapFile}/{path}";
        }


        public static T DeserializeFromFile<T>(string mapSetPath, string mapFile)
        {
            var path = RelativeToMap(mapSetPath, mapFile);
#if DEBUG
            GD.Print($"Loading {path}");
#endif
            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();
            var read = new File();
            read.Open(path, File.ModeFlags.Read);
            var res = deserializer.Deserialize<T>(read.GetAsText());
#if DEBUG
            GD.Print($"Done loading {path}");
#endif
            return res;
        }

        public static void SaveMap(object mapSet, string filePath)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(mapSet);
            var dir = new Directory();
            dir.MakeDir($"user://{filePath}");
            var save = new File();
            save.Open(filePath, File.ModeFlags.Write);
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

        public static string GetFileExtension(string fileName)
        {
            var ext = new FileInfo(fileName).Extension;
            // GD.Print(ext);
            return ext;
        }

        public static string GetFileName(string fileName)
        {
            var name = new FileInfo(fileName).Name;
            return WithoutExtension(name);
        }

        public static string WithoutExtension(string name)
        {
            return name.Substr(0, name.Length - GetFileExtension(name).Length);
        }

        public static Dictionary<string, string> ExtensionGameModeMap { get; } = new();
        public static Dictionary<string, string> GameModeExtensionMap { get; } = new();

        public static string GetGameModeFromExtension(string ext) => ExtensionGameModeMap[ext];
        public static string GetGameMode(string fileName)
        {
            return GetGameModeFromExtension(GetFileExtension(fileName));
        }

        public static string GetExtensionOfGameMode(string gameMode)
        {
            return GameModeExtensionMap[gameMode];
        }

        public static void RegisterExtension(string extension, string gameMode)
        {
            ExtensionGameModeMap.Add(extension, gameMode);
            GameModeExtensionMap.Add(gameMode, extension);
        }
    }
}