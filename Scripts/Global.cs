using System.Collections.Generic;
using System.IO;
using Godot;
using Quadrecep.GameMode.Keys;
using Quadrecep.GameMode.Navigate.Map;
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
        public const int AudioEffectPitchShiftIndex = 0;

        public static AudioEffectPitchShift AudioEffectPitchShift =>
            (AudioEffectPitchShift) AudioServer.GetBusEffect(1, AudioEffectPitchShiftIndex);

        public static Dictionary<string, string> ExtensionGameModeMap { get; } = new();

        public static Dictionary<string, string> GameModeExtensionMap { get; } = new();

        public override void _Ready()
        {
            Config.Initialize();
            // DatabaseHandler.Initialize();
            UpdateVideoConfig();
            Play.BaseSV = Config.NavigateScrollSpeed;
            UpdateAudioConfig();
            LoadPackedScenes();
            LoadTextures();
            GameModeInfo.Init();
            GameMode.Keys.GameModeInfo.Init();
        }

        private static void UpdateVideoConfig()
        {
            OS.VsyncEnabled = Config.VsyncEnabled;
            OS.WindowFullscreen = Config.WindowFullscreen;
            OS.WindowBorderless = Config.WindowBorderless;
        }

        /// <summary>
        /// Loads all packed scenes of nodes
        /// </summary>
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

        /// <summary>
        /// Loads textures required by game modes
        /// </summary>
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

        /// <summary>
        /// Loads an image from specified path.<br/>
        /// The image can be from res:// or from user://
        /// </summary>
        /// <param name="imgPath">The path to image</param>
        /// <param name="fallback">The fallback image path if <paramref name="imgPath"/> is not found</param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns absolute path relative to the map.<br/>
        /// This is used to get absolute path of paths specified in a map file.
        /// </summary>
        /// <param name="mapSetFile">MapSet file path</param>
        /// <param name="path">Relative path (without './')</param>
        /// <param name="absolutePath">If path returned is absolute or using "user://"</param>
        /// <returns>Path processed</returns>
        public static string RelativeToMap(string mapSetFile, string path = "", bool absolutePath = false)
        {
            var root = absolutePath ? OS.GetUserDataDir() : "user://";
            return $"{root}//{MapSet.MapDirectory}/{mapSetFile}/{path}";
        }


        /// <summary>
        /// Reads data from a file and deserialize to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <param name="mapSetPath">MapSet path</param>
        /// <param name="mapFile">Map file</param>
        /// <typeparam name="T">Type to deserialize into</typeparam>
        /// <returns>Deserialized object</returns>
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

        /// <summary>
        /// Serialize an object and save to a file
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <param name="filePath">Path to save</param>
        public static void SerializeToFile(object obj, string filePath)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(obj);
            var dir = new Directory();
            dir.MakeDir($"user://{filePath}");
            var save = new File();
            save.Open(filePath, File.ModeFlags.Write);
            save.StoreString(yaml);
            save.Close();
        }

        /// <summary>
        /// Swaps the textures of two <see cref="TextureRect"/>
        /// </summary>
        /// <param name="texture1">Texture 1</param>
        /// <param name="texture2">Texture 2</param>
        public static void SwapTexture(TextureRect texture1, TextureRect texture2)
        {
            (texture1.Texture, texture2.Texture) = (texture2.Texture, texture1.Texture);
        }

        private static void SwapModulate(CanvasItem texture1, CanvasItem texture2)
        {
            (texture1.Modulate, texture2.Modulate) = (texture2.Modulate, texture1.Modulate);
        }

        /// <summary>
        /// Returns the extension of a file (in format '.xxx')
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>The extension of the file</returns>
        public static string GetFileExtension(string fileName)
        {
            var ext = new FileInfo(fileName).Extension;
            // GD.Print(ext);
            return ext;
        }

        /// <summary>
        /// Returns the name of the file, cutting the extension and it's parent directories off
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>File name without extension and parent directories</returns>
        public static string GetFileName(string fileName)
        {
            var name = new FileInfo(fileName).Name;
            return WithoutExtension(name);
        }

        /// <summary>
        /// Cuts the extension off the given file
        /// </summary>
        /// <param name="name">file input</param>
        /// <returns>String with extension cut</returns>
        public static string WithoutExtension(string name)
        {
            return name.Substr(0, name.Length - GetFileExtension(name).Length);
        }

        /// <summary>
        /// Gets game mode name from given extension
        /// </summary>
        /// <param name="ext">Extension of a game mode</param>
        /// <returns>Game mode name</returns>
        public static string GetGameModeFromExtension(string ext)
        {
            return ExtensionGameModeMap[ext];
        }

        /// <summary>
        /// Gets game mode from a file. <br/>
        /// First gets the extension of the file then calls <see cref="GetGameModeFromExtension"/>
        /// </summary>
        /// <param name="fileName">File name to get game mode</param>
        /// <returns>The game mode name of the file</returns>
        public static string GetGameMode(string fileName)
        {
            return GetGameModeFromExtension(GetFileExtension(fileName));
        }

        /// <summary>
        /// Gets extension of a game mode
        /// </summary>
        /// <param name="gameMode">Game mode name</param>
        /// <returns>The extension of the game mode</returns>
        public static string GetExtensionOfGameMode(string gameMode)
        {
            return GameModeExtensionMap[gameMode];
        }

        /// <summary>
        /// Relates an extension with a game mode
        /// </summary>
        /// <param name="extension">Extension of a game mode</param>
        /// <param name="gameMode">Game mode name</param>
        public static void RegisterExtension(string extension, string gameMode)
        {
            ExtensionGameModeMap.Add(extension, gameMode);
            GameModeExtensionMap.Add(gameMode, extension);
        }

        public static void UpdateAudioConfig()
        {
            AudioEffectPitchShift.Oversampling = Config.TimeStretchOversampling;
            AudioEffectPitchShift.FftSize = (AudioEffectPitchShift.FFT_Size)Config.TimeStretchFftSize;
        }

        /// <summary>
        /// Sets the rate of the audio.<br/>
        /// Will keep the pitch same if pitch stretch is on
        /// </summary>
        /// <param name="player"></param>
        /// <param name="rate"></param>
        /// <param name="pitchStretch"></param>
        public static void UpdateRate(AudioStreamPlayer player, float rate, bool pitchStretch)
        {
            player.PitchScale = rate;
            if (!pitchStretch)
                ((AudioEffectPitchShift) AudioServer.GetBusEffect(1, 0)).PitchScale = 1 / rate;
        }
    }
}