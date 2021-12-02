using Quadrecep.Gameplay;

namespace Quadrecep.GameMode.Keys
{
    public class InputRetriever : AInputRetriever
    {
        public override int Keys { get; set; } = 4;
        public override string InputName => $"keys{Keys}";
    }
}