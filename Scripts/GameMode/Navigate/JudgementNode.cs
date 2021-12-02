using System;
using Godot;
using Quadrecep.Gameplay;

namespace Quadrecep.GameMode.Navigate
{
    public class JudgementNode : Node2D
    {
        public static PackedScene Scene;
        private static Texture[] _textures;

        public static void LoadTextures()
        {
            _textures = new Texture[Enum.GetNames(typeof(Judgement)).Length];
            for (var i = 0; i < _textures.Length; i++)
                _textures[i] = Global.LoadImage($"res://Textures/Judgements/J{i}.png");
        }

        public override void _Ready()
        {
            var animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            animationPlayer.GetAnimation("Pop").TrackSetKeyValue(0, 0, GlobalPosition);
            animationPlayer.GetAnimation("Pop").TrackSetKeyValue(0, 1, GlobalPosition + new Vector2(0, -50));
            animationPlayer.Play("Pop");
        }

        public void SetJudgement(Judgement judgement)
        {
            GetNode<Sprite>("Sprite").Texture = _textures[(int) judgement];
        }

        public void _OnAnimationFinished(string n)
        {
            QueueFree();
        }
    }
}