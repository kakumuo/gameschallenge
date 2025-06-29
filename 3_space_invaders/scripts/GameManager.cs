using Godot;
using System;
using System.Net;

namespace SpaceInvadersNS
{
    public partial class GameManager : Node2D
    {
        Control uiGroup;
        Panel gameOverGroup; 
        Node2D enemyGroup;
        PlayerController player;
        PackedScene enemyGroupRes; 

        int points = 0;
        int remLives = 3;
        private DateTime enemyMoveTS;
        private TimeSpan enemyMoveThresh;
        private int enemyMoveHori = 0;
        private bool moveLeft = false;
        private const float SPEED_INC_FACTOR = 1.05f;
        private int enemyCount = 24; 

        public override void _Ready()
        {
            uiGroup = GetNode<Control>("ui_group");
            player = GetNode<PlayerController>("player_ship");
            gameOverGroup = GetNode<Panel>("game_over_group");

            gameOverGroup.GetNode<Button>("retry_button").Pressed += resetGame; 

            player.OnPlayerFire += () =>
            {
                points = Math.Max(points - 1, 0);
                updateUI();
            };

            player.OnPlayerDamaged += () =>
            {
                remLives -= 1;
                if (remLives == 0)
                    doGameOver();
                updateUI();
            };

            enemyGroupRes = ResourceLoader.Load<PackedScene>("res://3_space_invaders/assets/enemy_group.tscn");
            resetGame(); 
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

        public void doGameOver()
        {
            GetTree().Paused = true; 
            gameOverGroup.Visible = true; 
        }


        // todo: clear out all bullets when game is reset
        // todo: set enemy group move rate back to default
        public void resetGame()
        {
            gameOverGroup.Visible = false;
            GetTree().Paused = false;

            if (enemyGroup != null)
            {
                enemyGroup.Free();
            }

            foreach(var bullet in GetTree().Root.GetChildren()){
                if (bullet is BulletBehavior) bullet.Free();  
            }

            enemyGroup = enemyGroupRes.Instantiate<Node2D>();
            foreach (EnemyBehavior enemy in enemyGroup.GetChildren())
            {
                enemy.OnEnemyKilled += () =>
                {
                    points += 5;
                    enemyCount--;

                    if (enemyCount == 0)
                        doGameOver();
                    updateUI();
                    enemyMoveThresh /= SPEED_INC_FACTOR;
                };
            }
            AddChild(enemyGroup);
            points = 0;
            remLives = 3;

            updateUI();
            enemyMoveTS = DateTime.Now;
            enemyMoveThresh = new TimeSpan(10000_000);
        }
    }
}