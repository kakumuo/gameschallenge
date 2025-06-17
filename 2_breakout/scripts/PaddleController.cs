using BreakoutNamespace;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class PaddleController : CharacterBody2D
{
    [Export]
    float moveSpeed = 400f;
    Dictionary<string, Key> inputMap;
    BallBehavior targetBall;
    const float BALL_GRAB_OFFSET = 40f;

    public override void _Ready()
    {
        inputMap = new Dictionary<string, Key>
        {
            {"left", Key.A},
            {"right", Key.D},
            {"up", Key.W}
        };
    }

    public override void _Process(double delta)
    {
        int moveDir = 0;

        if (Input.IsKeyPressed(inputMap["left"])) moveDir = 1;
        if (Input.IsKeyPressed(inputMap["right"])) moveDir = -1;
        if (Input.IsKeyPressed(inputMap["up"])) this.ReleaseBall(Vector2.Up); 
        MoveAndCollide(moveDir * Vector2.Left * moveSpeed * ((float)delta));

        if (targetBall != null)
        {
            Vector2 ballPos = targetBall.Position;
            ballPos.X = Position.X;
            ballPos.Y = Position.Y - BALL_GRAB_OFFSET;
            targetBall.Position = ballPos; 
        }
    }

    public void GrabBall(BallBehavior ball)
    {
        this.targetBall = ball;
        Vector2 ballPos = targetBall.Position;
        ballPos.X = Position.X;
        ballPos.Y = Position.Y - BALL_GRAB_OFFSET;
        targetBall.Position = ballPos; 
    }

    public void ReleaseBall(Vector2 launchVec)
    {
        if (this.targetBall != null)
        {
            this.targetBall.LaunchBall(launchVec); 
            this.targetBall = null; 
        }

    }
}

