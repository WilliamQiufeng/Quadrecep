using System;
using Godot;

namespace Quadrecep.GameMode.Keys
{
    public class Receptor : Node2D
    {
        public static PackedScene Scene;
        public int KeyIndex;
        private Texture _upTexture, _downTexture;
        private const string FallbackTexture = "res://Textures/Gamemodes/Keys/Receptors/receptor-up-1.png";
        public override void _Ready()
        {
            LoadTexture();
            GetNode<Sprite>("Sprite").Texture = _upTexture;
        }

        public Vector2 MaxSize()
        {
            return new Vector2(Math.Max(_upTexture.GetWidth(), _downTexture.GetWidth()),
                Math.Max(_upTexture.GetHeight(), _downTexture.GetHeight()));
        }

        public void LoadTexture()
        {
            _upTexture = Global.LoadImage($"res://Textures/Gamemodes/Keys/Receptors/receptor-up-{KeyIndex + 1}.png", FallbackTexture);
            _downTexture = Global.LoadImage($"res://Textures/Gamemodes/Keys/Receptors/receptor-down-{KeyIndex + 1}.png", FallbackTexture);
        }
    }
}