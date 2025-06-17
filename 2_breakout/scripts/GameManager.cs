using Godot;
using System;

namespace BreakoutNamespace
{
    public partial class GameManager : Node2D
    {
        BallBehavior ball;
        PaddleController paddle;
        Area2D outboundsArea;
        float areaPadding = 10f;

        public override void _Ready()
        {
            ball = GetNode<BallBehavior>("ball");
            paddle = GetNode<PaddleController>("paddle");
            outboundsArea = GetNode<Area2D>("outbounds_area");
            outboundsArea.BodyEntered += (body) =>
            {
                paddle.GrabBall(ball);
            };
            paddle.GrabBall(ball);
            InitBricks(); 
        }

        private void InitBricks()
        {
            // Vector2 windowDim = GetTree().Root.GetCamera2D().GetWindow().Size; 
        }

        public override void _Process(double delta)
        {

        }
    }

}
