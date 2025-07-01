using Godot;
using System;

namespace AsteroidsNamespce
{
    public partial class GameManager : Node2D
    {
        PlayerController player;
        Area2D gameArea;


        public override void _Ready()
        {
            player = GetNode<PlayerController>("player_ship");
            gameArea = GetNode<Area2D>("game_area"); 

            gameArea.BodyExited += (body) =>
            {
                var curPos = body.GlobalPosition;
                body.GlobalPosition = new Vector2(-curPos.X, -curPos.Y); 
            }; 
        }

        public override void _Process(double delta)
        {
            QueueRedraw();
        }

        public override void _Draw()
        {
            // draw player facing
            var curPlayerPos = player.GlobalPosition;
            Vector2 facingDir = player.getFacingDir(); 
            DrawLine(curPlayerPos, curPlayerPos + (facingDir * 100), Colors.Red); 
        }
    }
}

