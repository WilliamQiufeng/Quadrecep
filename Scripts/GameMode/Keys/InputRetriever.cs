using Quadrecep.GameMode.Keys.Map;

namespace Quadrecep.GameMode.Keys
{
    public class InputRetriever : AInputRetriever<NoteObject>
    {
        public override int Keys { get; set; } = 7;
        public override string InputName => $"keys{Keys}";
    }
}