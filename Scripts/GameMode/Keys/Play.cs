using Godot;
using Quadrecep.GameMode.Keys.Map;

namespace Quadrecep.GameMode.Keys
{
    public class Play : APlay<NoteObject>
    {
        protected override string BackgroundNodePath => "Background";
        protected override string InputProcessorPath => "InputProcessor";
        public static PackedScene Scene;
        protected MapObject MapObject;
        protected MapSet MapSet;

        protected string PlayfieldPath => "Playfield";
        public Playfield Playfield => GetNode<Playfield>(PlayfieldPath);

        protected override void ReadMap()
        {
            base.ReadMap();
            MapSet = new MapSet(MapSetFile);
            MapSet.ReadMap();
            MapObject = MapSet.GetMap(MapFile);
            ((InputProcessor) InputProcessor).LaneCount = MapObject.LaneCount;
        }

        protected override void SetParents()
        {
            base.SetParents();
            Playfield.Parent = this;
        }

        protected override void AfterReady()
        {
            base.AfterReady();
            Playfield.InitField();
        }
    }
}