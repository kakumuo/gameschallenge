using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BreakoutNamespace
{
    public partial class GameManager : Node2D
    {
        BallBehavior ball;
        PaddleController paddle;
        Area2D outboundsArea;
        ReferenceRect brickGroup; 
        float areaPadding = 10f;
        Camera2D camera;
        int playerScore;
        int remLives; 
        Control uiGroup;

        public override void _Ready()
        {
            ball = GetNode<BallBehavior>("ball");
            paddle = GetNode<PaddleController>("paddle");
            outboundsArea = GetNode<Area2D>("outbounds_area");
            camera = GetNode<Camera2D>("camera");
            uiGroup = GetNode<Control>("ui");
            brickGroup = GetNode<ReferenceRect>("brickGroup"); 

            outboundsArea.BodyEntered += (body) =>
            {
                ball.ballVel = Vector2.Zero;
                paddle.GrabBall(ball);
                remLives--;

                if (remLives == 0)
                {
                    uiGroup.GetNode<Panel>("game_over").Visible = true;
                    paddle.disableInput = true; 
                }
                updateUI();
            };

            ball.OnBrickCollision += (targetBrick) =>
            {
                brickGroup.RemoveChild(targetBrick); 
                targetBrick.Free();
                playerScore += (int)(ball.ballVel.DistanceTo(Vector2.Zero) / 100);
            };

            ball.OnVelocityChanged += (newVel) =>
            {
                updateUI();
            };

            uiGroup.GetNode<Panel>("game_over").GetNode<Button>("retry_btn").Pressed += () =>
            {
                uiGroup.GetNode<Panel>("game_over").Visible = false;
                resetGame(); 
            };

            resetGame(); 
        }

        private void resetGame()
        {
            foreach (BrickBehavior b in brickGroup.GetChildren())
            {
                b.Free();
            }

            remLives = 3;
            playerScore = 0; 

            paddle.disableInput = false; 
            paddle.GrabBall(ball);
            Vector2 viewportSize = camera.GetViewportRect().Size;
            InitBricks(10, 4, brickGroup.Size.X, brickGroup.Size.Y, 20, 10);
            updateUI();
        }

        
        private void InitBricks(int cols, int rows, float width, float height, float spacing, float padding)
        {
            PackedScene brickResource = ResourceLoader.Load<PackedScene>("res://2_breakout/assets/brick.tscn");

            (float rectWidth, float rectHeight) = (
                (width - (2 * padding) - (spacing * (cols - 1))) / cols,
                (height - (2 * padding) - (spacing * (rows - 1))) / rows
            );

            for (int colI = 0; colI < cols; colI++)
            {
                for (int rowI = 0; rowI < rows; rowI++)
                {
                    float xPos = padding + colI * spacing + colI * rectWidth + (rectWidth / 2);
                    float yPos = padding + rowI * spacing + rowI * rectHeight + (rectHeight / 2);
                    BrickBehavior brickObj = brickResource.Instantiate<BrickBehavior>();
                    brickObj.Name = "Brick " + rowI + " " + colI;  
                    brickObj.ZIndex = -2;

                    Vector2 brickPos = brickObj.Position;
                    brickPos.X = xPos;
                    brickPos.Y = yPos;
                    brickObj.Position = brickPos;

                    brickGroup.CallDeferred("add_child", brickObj);
                    brickObj.CallDeferred("setSize", [rectWidth, rectHeight]);
                }
            }
        }

        public void updateUI()
        {
            this.uiGroup.GetNode<Label>("livesGroupLabel").Text = "Lives: " + remLives.ToString(); 
            this.uiGroup.GetNode<Label>("scoreLabel").Text = playerScore.ToString();
            this.uiGroup.GetNode<Label>("ballSpeedLabel").Text = Math.Round(ball.ballVel.DistanceTo(Vector2.Zero) / 10).ToString() + " MPH"; 
        }
    }

}
