using System;
using Godot;
using Quadrecep.Gameplay;

namespace Quadrecep.GameMode.Keys
{
    public class JudgementNode : Node2D
    {
        public static PackedScene Scene;
        private static Texture[] _textures;

        private static readonly string[] TextureNames =
        {
            "marv", "perf", "great", "good", "okay", "miss"
        };

        private AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");

        public static void LoadTextures()
        {
            _textures = new Texture[Enum.GetNames(typeof(Judgement)).Length];
            for (var i = 0; i < _textures.Length; i++)
                _textures[i] =
                    Global.LoadImage($"res://Textures/GameModes/Keys/Judgements/judge-{TextureNames[i]}.png");
        }

        public override void _Ready()
        {
            Visible = false;
        }

        public void Animate()
        {
            Visible = true;
            AnimationPlayer.Stop();
            AnimationPlayer.Play("KeysJudgePop");
        }

        public void SetJudgement(Judgement judgement)
        {
            GetNode<Sprite>("Sprite").Texture = _textures[(int) judgement];
        }

        public void _OnAnimationFinished(string n)
        {
            Visible = false;
        }
    }
}