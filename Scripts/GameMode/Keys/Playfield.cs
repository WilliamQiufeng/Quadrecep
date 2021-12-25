using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Quadrecep.GameMode.Keys.Map;
using Quadrecep.Map;
using Path = Quadrecep.Gameplay.Path;

namespace Quadrecep.GameMode.Keys
{
    public class Playfield : CanvasLayer
    {
        public const float RealCoverHeight = 1080;
        private readonly NoteNodeChunkQueue _chunkQueue = new();

        private Vector2 _receptorSize = new(256, 277);
        private Vector2 _receptorsSize;

        public List<NoteNode> NoteNodes = new();
        public Play Parent;
        public float[] ReceptorX;
        private Vector2 VisibleRegionPos1 => new(-1579, PlayfieldNoteTopY - 100 - Parent.GlobalVisualOffset);
        private Vector2 VisibleRegionPos2 => new(2600.5f, 100 + Parent.GlobalVisualOffset);
        protected float PlayfieldNoteTopY => -RealCoverHeight * 4 + _receptorsSize.y;
        private Sprite BorderL => Cover.GetNode<Sprite>("BorderL");
        private Sprite BorderR => Cover.GetNode<Sprite>("BorderR");
        public Sprite Cover => GetNode<Sprite>("Main/Cover");

        private Sprite ReceptorsContainer => GetNode<Sprite>("Main/Receptors");

        public Node2D Notes => GetNode<Node2D>("Main/Receptors/Notes");

        public Receptor GetReceptor(int key)
        {
            return ReceptorsContainer.GetChild<Receptor>(key + 1); // Notes Node are at top
        }

        public override void _Process(float delta)
        {
            PullNoteNode();
            // if (!GenerationDone) return;
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
            ReceptorsContainer.Scale =
                new Vector2(Cover.Texture.GetWidth() * Cover.Scale.x / _receptorsSize.x,
                    0.45f);
            ReceptorsContainer.Position = new Vector2(Cover.Texture.GetWidth() * Cover.Scale.x / -2f,
                RealCoverHeight / 2f - _receptorSize.y * ReceptorsContainer.Scale.y);
            NoteNode.LoadTextures(Parent.InputRetriever.Keys);
        }


        public async Task GenerateNoteNodesAsync(List<NoteObject> notes, List<ScrollVelocity> svs)
        {
            GD.Print("Starting Generation");
            if (notes.Count == 0) return;
            List<Task> tasks = new();
            for (var i = 0; i < Parent.InputRetriever.Keys; i++)
            {
                var lane = i;
                tasks.Add(Task.Run(() => GenerateNoteNodesForLane(notes, svs, lane)));
            }

            await Task.WhenAll(tasks.ToArray());

            // NoteNodes = _tempNoteNodesOut.ToList();
            // NoteNodes.Sort((x, y) => x.Note.StartTime.CompareTo(y.Note.StartTime));
            GD.Print("Generation async done");
        }

        public void GenerateNoteNodesForLane(List<NoteObject> notes, List<ScrollVelocity> svs, int lane)
        {
            var pathCutoff = Config.NoteGenerationPathCutoff;
            var laneNotes =
                new Queue<NoteObject>(notes.Where(x => x.Lane == lane));
            var laneSVs = svs.Where(x => x.Key == -1 || x.Key == lane).Prepend(new ScrollVelocity(0, 1)).ToList();
            var svIndex = 0;
            var currentPosition = new Vector2(0, 0); // Origin
            var paths = new List<Path>();
            var lastSVFactor = 1.0f; // Initial SV
            var baseOffset = new Vector2((ReceptorX[lane] + ReceptorX[lane + 1]) / 2, 0);

            for (; svIndex < laneSVs.Count; svIndex++)
            {
                // If there are SV changes between [lastNoteStartTime...currentNoteStartTime), 
                // break the path into sections
                while (laneNotes.Count != 0 && (svIndex == laneSVs.Count - 1 ||
                                                svIndex < laneSVs.Count - 1 && laneNotes.Peek().EndTime <
                                                laneSVs[svIndex + 1].Time))
                {
                    NoteNode node;
                    lock (NoteNode.Scene)
                    {
                        node = NoteNode.Scene.Instance<NoteNode>();
                    }
                    var note = laneNotes.Dequeue();
                    if (note.CustomPaths != null && note.CustomPaths.Count != 0)
                    {
                        node.Paths = note.CustomPaths;
                    }
                    else
                    {
                        var path = new Path(Parent.BaseSV, Mathf.Abs(lastSVFactor), laneSVs[svIndex].Time,
                            note.EndTime,
                            new Vector2(0, lastSVFactor > 0 ? 1 : -1),
                            currentPosition, null);
                        node.Paths = paths.Append(path).ToList();
                    }

                    node.Paths = node.Paths.Select(x => x + baseOffset + -node.When(note.StartTime))
                        .ToList();

                    node.GenerateVisiblePaths(VisibleRegionPos1, VisibleRegionPos2 + node.Paths.Last().EndPosition);
                    node.Parent = this;
                    node.Note = note;
                    _chunkQueue.Enqueue(node);
                }

                // Add final path
                // If there isn't any SVs between, the start time will be lastNoteStartTime
                if (svIndex != laneSVs.Count - 1)
                {
                    paths.Add(new Path(Parent.BaseSV, Mathf.Abs(lastSVFactor), laneSVs[svIndex].Time,
                        laneSVs[svIndex + 1].Time, new Vector2(0, lastSVFactor > 0 ? 1 : -1),
                        currentPosition, null));
                    currentPosition = paths[paths.Count - 1].EndPosition;
                    while (paths.Count > pathCutoff && pathCutoff != -1) paths.RemoveAt(0);
                }

                lastSVFactor = laneSVs[svIndex].Factor;
                // GD.Print($"Lane {i} {svIndex}th SV");
            }

            GD.Print($"Lane {lane} done");
        }

        public void PullNoteNode()
        {
            _chunkQueue.Pull(Parent.Time, ref NoteNodes);
        }

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
                ReceptorsContainer.AddChild(receptor);
            }

            ReceptorX[Parent.InputRetriever.Keys] = _receptorsSize.x;

            _receptorSize.y = _receptorsSize.y;

            foreach (var r in ReceptorsContainer.GetChildren())
                if (r is Receptor receptor)
                    receptor.Position = new Vector2(receptor.Position.x, _receptorSize.y - receptor.MaxSize().y);
        }
    }
}