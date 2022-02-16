using System.Threading.Tasks;
using Godot;
using Quadrecep.GameMode.Keys.Map;

namespace Quadrecep.GameMode.Keys
{
    public class Play : APlay<NoteObject>
    {
        public static PackedScene Scene;
        public float BaseSV = 4500;
        public MapObject MapObject;
        public MapSet MapSet;
        protected override string BackgroundNodePath => "Background";
        protected override string InputProcessorPath => "InputProcessor";

        protected override string BackgroundPath => MapSet.MapSetObject.BackgroundPath;
        protected override string AudioPath => MapSet.MapSetObject.AudioPath;

        protected string PlayfieldPath => "Playfield";
        public Playfield Playfield => GetNode<Playfield>(PlayfieldPath);

        public override float DynamicTime => base.DynamicTime + GlobalOffset;

        protected override void ReadMap()
        {
            base.ReadMap();
            MapSet = new MapSet(MapSetFile);
            MapSet.ReadMapSet();
            BaseSV /= Rate;
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
            
            Task.Run(() => Playfield.SupplyTempNoteNode(MapObject.Notes.Count));
            Task.Run(() => Playfield.GenerateNoteNodesAsync(MapObject.Notes, MapObject.ScrollVelocities));
            GetNode<Label>("HUD/Name").Text = MapSet.MapSetObject.Name;
            base.AfterReady();
        }

        internal override void Retry()
        {
            var scene = Scene.Instance<Play>();
            scene.PassValuesFrom(this);
            scene.MapObject = MapObject;
            Global.SwitchScene(this, scene);
        }
    }
}