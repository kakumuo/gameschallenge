using Godot;
using System;
using System.Net;

namespace SpaceInvadersNS
{
    public partial class GameManager : Node
    {
        Control uiGroup;
        Node2D enemyGroup;
        PlayerController player; 

        int points = 0;
        int remLives = 3;
        private DateTime enemyMoveTS;
        private TimeSpan enemyMoveThresh;
        private int enemyMoveHori = 0;
        private bool moveLeft = false; 

        public override void _Ready()
        {
            uiGroup = GetNode<Control>("ui_group");
            enemyGroup = GetNode<Node2D>("enemy_group");
            player = GetNode<PlayerController>("player_ship");


            player.OnPlayerFire += () =>
            {
                points -= 4; 
                updateUI();
            };  

            foreach (EnemyBehavior enemy in enemyGroup.GetChildren())
            {
                enemy.OnEnemyKilled += () =>
                {
                    points += 5;
                    updateUI();
                };
            }

            updateUI();
            enemyMoveTS = DateTime.Now;
            enemyMoveThresh = new TimeSpan(10000_000);
        }

        public override void _Process(double delta)
        {
            if (DateTime.Now - enemyMoveTS < enemyMoveThresh) return;

            var targetPos = enemyGroup.Position;
            if (enemyMoveHori == 2 || enemyMoveHori == -2)
            {
                moveLeft = !moveLeft;
                targetPos += Vector2.Down * 10;
            }
            else
            {
                targetPos += (moveLeft ? Vector2.Right : Vector2.Left) * 10;
            }

            enemyMoveHori += moveLeft ? 1 : -1;
            enemyGroup.Position = targetPos;
            enemyMoveTS = DateTime.Now; 
        }

        public void updateUI()
        {
            uiGroup.GetNode<Label>("score_label").Text = "Score: " + points;
            uiGroup.GetNode<Label>("lives_label").Text = "Lives: " + remLives;
        }
    }
}