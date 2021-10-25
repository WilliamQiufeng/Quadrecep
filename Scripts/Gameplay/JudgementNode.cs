using Godot;

namespace Quadrecep.Gameplay
{
    public class JudgementNode : Node2D
    {
        public Judgement Judgement;
        public override void _Ready()
        {
            GetNode<Sprite>("Sprite").Texture = ResourceLoader.Load($"res://Textures/Judgements/J{(int)Judgement}.png") as Texture;
            var animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            animationPlayer.GetAnimation("Pop").TrackSetKeyValue(0, 0, GlobalPosition);
            animationPlayer.GetAnimation("Pop").TrackSetKeyValue(0, 1, GlobalPosition + new Vector2(0, -50));
            animationPlayer.Play("Pop");
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
        }

        public void _OnAnimationFinished(string n)
        {
            QueueFree();
        }
    }
}