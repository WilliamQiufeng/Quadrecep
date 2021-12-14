using Quadrecep.GameMode.Navigate.Map;

namespace Quadrecep.GameMode.Navigate
{
    public class InputRetriever : AInputRetriever<NoteObject>
    {
        public override int Keys => 4;
        public override string InputName => "navigate";
    }
}