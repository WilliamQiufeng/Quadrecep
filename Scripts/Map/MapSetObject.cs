using System.Collections.Generic;

namespace Quadrecep.Map
{
    public class MapSetObject
    {
        public float PreviewTime { get; set; }
        public string Name { get; set; } = "";
        public string Artist { get; set; } = "";
        public string Creator { get; set; } = "";
        public string Description { get; set; } = "";
        public string AudioPath { get; set; } = "audio.mp3";
        public string BackgroundPath { get; set; } = "background.jpg";
        public int LocalId { get; set; } = -1;
        public int OnlineId { get; set; } = -1;
        public List<string> Maps { get; set; } = new();

        public override string ToString()
        {
            return
                $"[MapSet: {nameof(PreviewTime)}: {PreviewTime}, {nameof(Name)}: {Name}, {nameof(Artist)}: {Artist}, {nameof(Creator)}: {Creator}, {nameof(Description)}: {Description}, {nameof(AudioPath)}: {AudioPath}, {nameof(BackgroundPath)}: {BackgroundPath}, {nameof(LocalId)}: {LocalId}, {nameof(OnlineId)}: {OnlineId}, {nameof(Maps)}: {Maps}]";
        }
    }
}