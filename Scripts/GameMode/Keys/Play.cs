using Godot;
using Quadrecep.GameMode.Keys.Map;

namespace Quadrecep.GameMode.Keys
{
    public class Play : APlay<NoteObject>
    {
        public static PackedScene Scene;
        public float BaseSV = 5000;
        public MapObject MapObject;
        public MapSet MapSet;
        protected override string BackgroundNodePath => "Background";
        protected override string InputProcessorPath => "InputProcessor";

        protected override string BackgroundPath => MapSet.MapSetObject.BackgroundPath;
        protected override string AudioPath => MapSet.MapSetObject.AudioPath;

        protected string PlayfieldPath => "Playfield";
        public Playfield Playfield => GetNode<Playfield>(PlayfieldPath);

        protected override void ReadMap()
        {
            base.ReadMap();
            MapSet = new MapSet(MapSetFile);
            MapSet.ReadMap();
            ((InputProcessor) InputProcessor).LaneCount = MapObject.LaneCount;
            InputRetriever.Keys = MapObject.LaneCount;
        }

        protected override void FeedNotes()
        {
            base.FeedNotes();
            ((InputProcessor) InputProcessor).FeedNotes(MapObject.Notes);
        }

        protected override void SetParents()
        {
            base.SetParents();
            Playfield.Parent = this;
        }

        protected override void AfterReady()
        {
            GD.Print("Initializing Field");
            Playfield.InitField();
            GD.Print("Generating Nodes");
            Playfield.GenerateNoteNodesAsync(MapObject.Notes, MapObject.ScrollVelocities);
            // while (!Playfield.GenerationDone) ;
            GD.Print("Done");
            base.AfterReady();
        }
    }
}