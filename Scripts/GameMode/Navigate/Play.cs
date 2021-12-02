using System.Collections.Generic;
using Godot;
using Quadrecep.Map;
using static Godot.Vector2;
using Path = Quadrecep.Map.Path;

namespace Quadrecep.GameMode.Navigate
{
    public class Play : APlay
    {
        public static PackedScene Scene;

        private int _approachingPathIndex;
        private int _pathIndex;
        private Queue<NoteNode> _nodePool = new();

        public Path CurrentPath => MapObject.Paths[_pathIndex];

        protected override void AfterReady()
        {
            for (var i = 0; i < MapObject.Paths.Count; i++)
            {
                var path = MapObject.Paths[i];
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
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            UpdateCurrentPath();
            PlaceApproachingNotes();
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
                        Mathf.Rad2Deg(GetNoteRotation(CurrentPath.TargetNote.Direction)), 0.1f / CurrentPath.Factor);
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