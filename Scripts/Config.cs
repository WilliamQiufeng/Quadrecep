using Godot;
using Path = Quadrecep.Map.Path;

namespace Quadrecep
{
    public class Config
    {
        public static ConfigFile File { get; } = new();
        public const string ConfigPath = "user://config.cfg";
        private const string GameplaySection = "Gameplay";
        private const string VideoSection = "Video";
        private const string ScrollSpeedSection = "ScrollSpeed";

        public static float NavigateScrollSpeed
        {
            get => GetOrSet(GameplaySection, nameof(NavigateScrollSpeed), 500f);
            set
            {
                Set(ScrollSpeedSection, nameof(NavigateScrollSpeed), value);
                Path.BaseSV = value;
            }
        }

        public static bool VsyncEnabled
        {
            get => GetOrSet(VideoSection, nameof(VsyncEnabled), false);
            set
            {
                Set(VideoSection, nameof(VsyncEnabled), value);
                OS.VsyncEnabled = value;
            }
        }

        public static bool WindowFullscreen
        {
            get => GetOrSet(VideoSection, nameof(WindowFullscreen), false);
            set
            {
                Set(VideoSection, nameof(WindowFullscreen), value);
                OS.WindowFullscreen = value;
            }
        }

        public static bool WindowBorderless
        {
            get => GetOrSet(VideoSection, nameof(WindowBorderless), false);
            set
            {
                Set(VideoSection, nameof(WindowBorderless), value);
                OS.WindowBorderless = value;
            }
        }

        public static float PlayfieldWidthPerKey
        {
            get => GetOrSet(GameplaySection, nameof(PlayfieldWidthPerKey), 1f / 6);
            set => Set(GameplaySection, nameof(PlayfieldWidthPerKey), value);
        }

        public static void Initialize()
        {
            Load();
        }

        private static void Load()
        {
            File.Load(ConfigPath);
        }

        private static void Save()
        {
            File.Save(ConfigPath);
        }

        private static T GetOrSet<T>(string section, string key, T fallback = default)
        {
            if (!File.HasSectionKey(section, key)) Set(section, key, fallback);
            return (T) File.GetValue(section, key);
        }

        private static void Set<T>(string section, string key, T value)
        {
            File.SetValue(section, key, value);
            Save();
        }
    }
}