using Godot;
using System;
using System.Linq;
using System.Reflection;

namespace AsteroidsNamespce
{
    public partial class GameManager : Node2D
    {
        int score = 0;
        int difficultyScaling = 1;
        DateTime lastDifficultyInc;
        TimeSpan difficultyIncSpan; 

        DateTime lastScoreUpdate;
        TimeSpan scoreUpdateSpan; 

        PlayerController player;
        Area2D gameArea;
        Control uiGroup;
        PathFollow2D spawnPath;

        DateTime lastSpawnTS;
        TimeSpan initialSpawnRate;
        TimeSpan spawnRateSpan;

        public override void _Ready()
        {
            player = GetNode<PlayerController>("player_ship");
            gameArea = GetNode<Area2D>("game_area");
            uiGroup = GetNode<Control>("ui");
            spawnPath = GetNode<Path2D>("asteroidSpawnPath").GetNode<PathFollow2D>("follow");

            player.OnPlayerKilled += () =>
            {
                GD.Print("Player killed"); 
                //TODO: add game resetting and kill screen
            }; 

            // init ui
            UpdateUI();

            gameArea.AreaExited += (area) =>
            {
                if (area is PlayerController)
                {
                    var curPos = area.GlobalPosition;
                    area.GlobalPosition = new Vector2(-curPos.X, -curPos.Y);
                }
                else
                {
                    area.QueueFree();
                }
            };

            // respawn
            lastSpawnTS = DateTime.Now;
            initialSpawnRate = new TimeSpan(2000 * 10_000);
            spawnRateSpan = new TimeSpan(2000 * 10_000);

            // difficulty; 
            lastDifficultyInc = DateTime.Now;
            difficultyIncSpan = new TimeSpan(5000 * 10_000);

            // points
            lastScoreUpdate = DateTime.Now;
            scoreUpdateSpan = new TimeSpan(1000 * 10_000); 
        }

        public override void _Process(double delta)
        {
            UpdateUI();
            QueueRedraw();

            if (DateTime.Now - lastDifficultyInc > difficultyIncSpan)
            {
                difficultyScaling += 1;
                lastDifficultyInc = DateTime.Now;
                spawnRateSpan = initialSpawnRate / (1 + (difficultyScaling * .1f)); 
            }

            if (DateTime.Now - lastScoreUpdate > scoreUpdateSpan)
            {
                score += difficultyScaling * 5;
                lastScoreUpdate = DateTime.Now; 
            }

            if (DateTime.Now - lastSpawnTS > spawnRateSpan)
            {
                spawnPath.ProgressRatio = GD.Randf();
                // FIXME: respawning asteroids changes asteroid size
                PackedScene asteroidRes = ResourceLoader.Load<PackedScene>("res://4_asteroids/assets/asteroid.tscn");
                AsteroidBehavior asteroidObj = asteroidRes.Instantiate<AsteroidBehavior>();
                GetTree().Root.AddChild(asteroidObj);
                asteroidObj.spawn((int)Math.Floor(GD.Randf() * 3), spawnPath.Position, (player.Position - spawnPath.Position).Normalized() * (1 + (difficultyScaling * .01f)));
                asteroidObj.OnAsteroidDestroyed += (asteroidType) =>
                {
                    score += (asteroidType + 1) * difficultyScaling * 100; 
                }; 
                lastSpawnTS = DateTime.Now;
            }
        }

        public void UpdateUI()
        {
            // init ui
            if (player != null)
            {
                uiGroup.GetNode<Label>("bulletsLabel").Text = String.Concat(Enumerable.Repeat("[x]", player.curAmmo));
                uiGroup.GetNode<Label>("healthLabel").Text = String.Concat(Enumerable.Repeat("❤️", player.remHealth));
                uiGroup.GetNode<Label>("scoreLabel").Text = "Score: " + score.ToString().PadLeft(10, '0'); 
                if (player.reloadTimer.isEnabled)
                {
                    uiGroup.GetNode<ProgressBar>("reloadProgress").MaxValue = 1;
                    uiGroup.GetNode<ProgressBar>("reloadProgress").Value = player.reloadTimer.getCompletionRatio();
                }
            }   
        }

        public override void _Draw()
        {
            // draw player facing
            var curPlayerPos = player.GlobalPosition;
            Vector2 facingDir = player.GetFacingDir();
            DrawLine(curPlayerPos, curPlayerPos + (facingDir * 100), Colors.Red);
        }
    }
}

