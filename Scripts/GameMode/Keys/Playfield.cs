using System;
using System.Collections.Generic;
using Godot;

namespace Quadrecep.GameMode.Keys
{
    public class Playfield : CanvasLayer
    {
        public const float RealCoverHeight = 600;

        public readonly List<NoteNode> NoteNodes = new();

        private Vector2 _receptorSize = new(256, 277);
        private Vector2 _receptorsSize;
        public Play Parent;
        public float[] ReceptorX;
        protected float PlayfieldNoteTopY => -RealCoverHeight * 4 + _receptorsSize.y;
        private Sprite BorderL => Cover.GetNode<Sprite>("BorderL");
        private Sprite BorderR => Cover.GetNode<Sprite>("BorderR");
        public Sprite Cover => GetNode<Sprite>("Main/Cover");

        private Sprite Receptors => GetNode<Sprite>("Main/Receptors");

        public Node2D Notes => GetNode<Node2D>("Main/Receptors/Notes");

        public override void _Process(float delta)
        {
            for (var i = 0; i < NoteNodes.Count; i++)
            {
                NoteNodes[i].CheckVisible();
                if (!NoteNodes[i].Finished) continue;
                NoteNodes.RemoveAt(i);
                i--;
            }
        }

        public void InitField()
        {
            ReceptorX = new float[Parent.InputRetriever.Keys + 1];
            // 0.167F: 2 / 3 / 4 = 1 / 6, 2 / 3 for a 4K playfield by default
            Cover.Scale = new Vector2(Parent.InputRetriever.Keys * Config.PlayfieldWidthPerKey,
                RealCoverHeight / Cover.Texture.GetHeight());
            BorderL.Scale = new Vector2(1, RealCoverHeight / BorderL.Texture.GetHeight() / Cover.Scale.y);
            BorderL.Position = new Vector2(Cover.Texture.GetWidth() / -2f - BorderL.Texture.GetWidth() / 2f, 0);
            BorderR.Scale = new Vector2(1, RealCoverHeight / BorderR.Texture.GetHeight() / Cover.Scale.y);
            BorderR.Position = new Vector2(Cover.Texture.GetWidth() / 2f + BorderR.Texture.GetWidth() / 2f, 0);
            PlaceReceptors();
            Receptors.Scale =
                new Vector2(Cover.Texture.GetWidth() * Cover.Scale.x / _receptorsSize.x,
                    0.25f);
            Receptors.Position = new Vector2(Cover.Texture.GetWidth() * Cover.Scale.x / -2f,
                RealCoverHeight / 2f - _receptorSize.y * Receptors.Scale.y);
            DebugPlaceNote(1, 0);
            DebugPlaceNote(4, PlayfieldNoteTopY);
        }

        private void DebugPlaceNote(int key, float height)
        {
            var note = NoteNode.Scene.Instance<NoteNode>();
            note.Parent = this;
            note.Key = key;
            note.Position = new Vector2(note.XPositionBase, height);
            Notes.AddChild(note);
        }

        public void GenerateNoteNodes()
        {
            
        }
        // /// <summary>
        // ///     note(t) #     #
        // ///     sv  (t)   # #
        // /// </summary>
        // public void BuildPaths()
        // {
        //     if (Notes.Count == 0) return;
        //     var svIndex = 0;
        //     var lastNoteStartTime = 0f; // This can be changed to be the audio offset
        //     var currentPosition = new Vector2(0, 0); // Origin
        //     var lastSVFactor = 1.0f; // Initial SV
        //     DirectionObject direction = 0b0010; // Up
        //
        //     foreach (var note in Notes)
        //     {
        //         UpdateSVIndex(ref svIndex, lastNoteStartTime);
        //         var slicedStartTime = lastNoteStartTime;
        //         // If there are SV changes between [lastNoteStartTime...currentNoteStartTime), 
        //         // break the path into sections
        //         for (;
        //             svIndex < ScrollVelocities.Count && ScrollVelocities[svIndex].Time < note.StartTime;
        //             svIndex++)
        //         {
        //             ref var endTime = ref ScrollVelocities[svIndex].Time;
        //             var path = new Path(Play.BaseSV, lastSVFactor, slicedStartTime, endTime, direction,
        //                 currentPosition, null);
        //             if (!slicedStartTime.Equals(endTime)) Paths.Add(path);
        //             lastSVFactor = ScrollVelocities[svIndex].Factor;
        //             slicedStartTime = endTime;
        //             currentPosition = path.EndPosition;
        //         }
        //
        //         // Add final path where the next note is `note`.
        //         // If there isn't any SVs between, the start time will be lastNoteStartTime
        //         Paths.Add(new Path(Play.BaseSV, lastSVFactor, slicedStartTime, note.StartTime, direction,
        //             currentPosition, note));
        //
        //         lastNoteStartTime = note.StartTime;
        //         currentPosition = Paths[Paths.Count - 1].EndPosition;
        //         direction = note.Direction;
        //     }
        // }
        //
        // private void UpdateSVIndex(ref int svIndex, float leastTime)
        // {
        //     while (svIndex < ScrollVelocities.Count && ScrollVelocities[svIndex].Time < leastTime) svIndex++;
        // }

        public void PlaceReceptors()
        {
            for (var i = 0; i < Parent.InputRetriever.Keys; i++)
            {
                var receptor = Receptor.Scene.Instance<Receptor>();
                receptor.Parent = this;
                receptor.KeyIndex = i;
                receptor.LoadTexture();
                receptor.Position = new Vector2(_receptorsSize.x, 0);
                ReceptorX[i] = _receptorsSize.x;
                _receptorsSize.x += receptor.MaxSize().x;
                _receptorSize.x = Math.Max(_receptorSize.x, receptor.MaxSize().x);
                _receptorsSize.y = Math.Max(_receptorsSize.y, receptor.MaxSize().y);
                Receptors.AddChild(receptor);
            }

            ReceptorX[Parent.InputRetriever.Keys] = _receptorsSize.x;

            _receptorSize.y = _receptorsSize.y;

            foreach (var r in Receptors.GetChildren())
                if (r is Receptor receptor)
                    receptor.Position = new Vector2(receptor.Position.x, _receptorSize.y - receptor.MaxSize().y);
        }
    }
}