using System.Collections.Generic;
using System.Linq;
using Godot;
using Quadrecep.GameMode.Keys.Map;
using Path = Quadrecep.Gameplay.Path;

namespace Quadrecep.GameMode.Keys
{
    public class NoteNode : Sprite
    {
        private static List<Texture[]> _hitObjectTextures = new() {null};
        private static List<Texture[]> _holdBodyTextures = new() {null};
        private static List<Texture[]> _holdEndTextures = new() {null};
        private static List<Texture[]> _holdHitObjectTextures = new() {null};

        public static PackedScene Scene;
        private readonly List<Path> _visiblePaths = new();

        private bool _hasParent;
        public NoteObject Note;
        public Playfield Parent;
        public List<Path> Paths = new();
        public float FirstAppearanceTime = -1;
        private int Key => Note.Lane;

        public float XPositionBase => (Parent.ReceptorX[Key] + Parent.ReceptorX[Key + 1]) / 2;
        public bool Finished => _visiblePaths.Count == 0;
        private Sprite HoldEnd => GetNode<Sprite>("HoldEnd");
        private Sprite HoldBody => GetNode<Sprite>("HoldBody");

        public override void _Ready()
        {
            Texture = _hitObjectTextures[Parent.Parent.InputRetriever.Keys][Key];
            Offset = new Vector2(-Texture.GetWidth() / 2f, -Texture.GetHeight());
            if (Note.IsLongNote)
            {
                HoldEnd.Texture = _holdEndTextures[Parent.Parent.InputRetriever.Keys][Key];
                HoldEnd.Offset = new Vector2(-HoldEnd.Texture.GetWidth() / 2f, -HoldEnd.Texture.GetHeight());
                var holdEndPosition = When(Note.StartTime) - When(Note.EndTime);
                HoldEnd.Position = holdEndPosition;
                HoldEnd.Visible = true;
                HoldBody.Texture = _holdBodyTextures[Parent.Parent.InputRetriever.Keys][Key];
                HoldBody.Offset = new Vector2(-HoldBody.Texture.GetWidth() / 2f, -HoldBody.Texture.GetHeight());
                HoldBody.Visible = true;
                HoldBody.Scale = new Vector2(1, Mathf.Abs(holdEndPosition.y) / HoldBody.Texture.GetHeight());
            }
        }

        public override void _Process(float delta)
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (_hasParent && _visiblePaths.Count != 0)
                Position = _visiblePaths[0][Parent.Parent.Time] + new Vector2(0, Parent.Parent.GlobalVisualOffset);
        }

        public void GenerateVisiblePaths(Vector2 regionPos1, Vector2 regionPos2)
        {
            _visiblePaths.Clear();
            foreach (var visiblePath in Paths.Select(path => Path.CutVisiblePath(path, regionPos1, regionPos2, false))
                .Where(visiblePath => visiblePath != null)) _visiblePaths.Add(visiblePath);
            if (_visiblePaths.Count != 0) FirstAppearanceTime = _visiblePaths[0].StartTime;
        }

        public Vector2 When(float time)
        {
            return Paths.FirstOrDefault(path => path.WithinTime(time))?.GetPosition(time) ?? Vector2.Zero;
        }

        public void CheckVisible()
        {
            if (RemoveIfFinished()) return;

            while (Parent.Parent.Time > _visiblePaths[0].EndTime)
            {
                _visiblePaths.RemoveAt(0);
                if (RemoveIfFinished()) return;
            }

            if (Parent.Parent.Time < _visiblePaths[0].StartTime)
            {
                RemoveFromParent();
                return;
            }

            AddToParent();
        }

        private bool RemoveIfFinished()
        {
            if (!Finished) return false;
            RemoveFromParent();
            return true;
        }

        private void AddToParent()
        {
            if (_hasParent) return;
            _hasParent = true;
            Parent.Notes.AddChild(this);
            Visible = true;
            UpdatePosition();
        }

        private void RemoveFromParent()
        {
            if (!_hasParent) return;
            Parent.Notes.RemoveChild(this);
            _hasParent = false;
            Visible = false;
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
                to[keys][i] =
                    Global.LoadImage(ImgPath(keys, name, i + 1), ImgPath(4, name, 1));
        }

        private static string ImgPath(int keys, string name, int i)
        {
            return $"{Global.TexturesPath}/GameModes/Keys/Keys{keys}/HitObjects/note-{name}-{i}.png";
        }

        private static void FillTexturesUpTo(int keys, ref List<Texture[]> to)
        {
            for (var i = to.Count; i <= keys; i++) to.Add(new Texture[i]);
        }

        public void ClearInput()
        {
            _visiblePaths.Clear();
            RemoveFromParent();
        }
    }
}