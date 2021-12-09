using System;
using Godot;

namespace Quadrecep.GameMode.Keys
{
    public class Playfield : CanvasLayer
    {
        public const float RealCoverHeight = 600;

        private Vector2 _receptorSize = new(256, 277);
        private Vector2 _receptorsSize;
        public float[] ReceptorX;
        public Play Parent;
        protected float PlayfieldNoteTopY => -RealCoverHeight * 4 + _receptorsSize.y;
        private Sprite BorderL => Cover.GetNode<Sprite>("BorderL");
        private Sprite BorderR => Cover.GetNode<Sprite>("BorderR");
        public Sprite Cover => GetNode<Sprite>("Main/Cover");

        private Sprite Receptors => GetNode<Sprite>("Main/Receptors");

        private Node2D Notes => GetNode<Node2D>("Main/Receptors/Notes");

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