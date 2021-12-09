using System.Collections.Generic;
using Godot;

namespace Quadrecep.GameMode.Keys
{
    public class NoteNode : Sprite
    {
        private static List<Texture[]> _hitObjectTextures = new() {null};
        private static List<Texture[]> _holdBodyTextures = new() {null};
        private static List<Texture[]> _holdEndTextures = new() {null};
        private static List<Texture[]> _holdHitObjectTextures = new() {null};

        public static PackedScene Scene;

        public float XPositionBase => (Parent.ReceptorX[Key] + Parent.ReceptorX[Key + 1]) / 2;
        public Playfield Parent;
        public int Key;

        public override void _Ready()
        {
            Texture = _hitObjectTextures[Parent.Parent.InputRetriever.Keys][Key];
            Offset = new Vector2(-Texture.GetWidth() / 2f, -Texture.GetHeight());
        }

        public static void LoadTextures(int keys)
        {
            FillTexturesUpTo(keys, ref _hitObjectTextures);
            LoadTexturesForKey(keys, "hitobject", ref _hitObjectTextures);
            FillTexturesUpTo(keys, ref _holdBodyTextures);
            LoadTexturesForKey(keys, "holdbody", ref _holdBodyTextures);
            FillTexturesUpTo(keys, ref _holdEndTextures);
            LoadTexturesForKey(keys, "holdend", ref _holdEndTextures);
            FillTexturesUpTo(keys, ref _holdHitObjectTextures);
            LoadTexturesForKey(keys, "holdhitobject", ref _holdHitObjectTextures);
        }

        private static void LoadTexturesForKey(int keys, string name, ref List<Texture[]> to)
        {
            for (var i = 0; i < keys; i++)
            {
                to[keys][i] =
                    Global.LoadImage(ImgPath(keys, name, i + 1), ImgPath(4, name, 1));
            }
        }

        private static string ImgPath(int keys, string name, int i)
        {
            return $"{Global.TexturesPath}/Gamemodes/Keys/Keys{keys}/HitObjects/note-{name}-{i}.png";
        }

        private static void FillTexturesUpTo(int keys, ref List<Texture[]> to)
        {
            for (var i = to.Count; i <= keys; i++)
            {
                to.Add(new Texture[i]);
            }
        }
    }
}