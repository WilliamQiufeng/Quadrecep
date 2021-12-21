using System.Collections.Generic;
using Godot;
using Quadrecep.GameMode.Navigate.Map;
using static Godot.Vector2;
using Path = Quadrecep.Gameplay.Path;

namespace Quadrecep.GameMode.Navigate
{
    public class Play : APlay<NoteObject>
    {
        public static PackedScene Scene;

        public static float BaseSV = 500;
        private readonly Queue<NoteNode> _nodePool = new();

        private int _approachingPathIndex;
        private int _pathIndex;
        public MapObject MapObject;
        protected MapSet MapSet;

        public Path CurrentPath => MapObject.Paths[_pathIndex];

        protected override string BackgroundPath => MapSet.MapSetObject.BackgroundPath;
        protected override string AudioPath => MapSet.MapSetObject.AudioPath;

        protected override void AfterReady()
        {
            GetNode<Label>("HUD/Name").Text = MapSet.MapSetObject.Name;
            foreach (var path in MapObject.Paths)
            {
                if (path.TargetNote == null) continue;
                var noteSprite = NoteNode.Scene.Instance<NoteNode>();
                noteSprite.Parent = this;
                noteSprite.Note = path.TargetNote;
                noteSprite.GlobalPosition = path.EndPosition;
                var targetNoteDirection = (DirectionObject) path.TargetNote.Direction;
                noteSprite.Rotation = GetNoteRotation(targetNoteDirection);
                noteSprite.GetNode<Node2D>("Side").Visible = targetNoteDirection.HasSide();
                noteSprite.ZIndex = ZInd--;
                _nodePool.Enqueue(noteSprite);
            }

            base.AfterReady();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            UpdateCurrentPath();
            PlaceApproachingNotes();
        }

        protected override void ReadMap()
        {
            base.ReadMap();
            MapSet = new MapSet(MapSetFile);
            MapSet.ReadMapSet();
            MapObject.BuildPaths();
        }

        protected override void FeedNotes()
        {
            base.FeedNotes();
            InputProcessor.FeedNotes(MapObject.Notes);
        }

        private void UpdateCurrentPath()
        {
            if (Finished) return;
            while (CurrentPath.EndTime < Time)
            {
                var player = GetNode<Player>("Player/Player");
                player.RectPosition = CurrentPath.EndPosition;
                if (CurrentPath.TargetNote != null)
                {
                    var tween = player.GetNode<Tween>("Tween");
                    tween.InterpolateProperty(player, "rect_rotation", player.RectRotation,
                        Mathf.Rad2Deg(GetNoteRotation(CurrentPath.TargetNote.Direction)),
                        Mathf.Pow(0.1f, CurrentPath.Factor));
                    tween.Start();
                }

                _pathIndex++;
                if (_pathIndex < MapObject.Paths.Count) continue;
                Finished = true;
                return;
            }
        }

        private void PlaceApproachingNotes()
        {
            while (_approachingPathIndex < MapObject.Paths.Count &&
                   MapObject.Paths[_approachingPathIndex].StartTime - Time <= NoteNode.FadeInTime)
            {
                var path = MapObject.Paths[_approachingPathIndex++];
                if (path.TargetNote == null) continue;
                var noteSprite = _nodePool.Dequeue();
                GetNode("Notes").AddChild(noteSprite);
            }
        }

        private static float GetNoteRotation(DirectionObject targetNoteDirection)
        {
            return Zero.AngleToPoint(targetNoteDirection.NetDirection) - Mathf.Pi / 2;
        }
    }
}