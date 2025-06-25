using Godot;
using System;
using System.Collections.Generic;

namespace SpaceInvadersNS
{
    public partial class PlayerController : StaticBody2D
    {

        [Export] float moveSpeed = 5;

        private Dictionary<String, Key> inputMap;
        private bool disableInput;
        PackedScene bulletRes;
        private bool didFire = false;
        private DateTime lastFireTS;
        private TimeSpan FIRE_THRESH = new TimeSpan(5000_000); 

        public override void _Ready()
        {
            inputMap = new Dictionary<string, Key>
            {
                {"left", Key.A},
                {"right", Key.D},
                {"shoot", Key.W}
            };

            disableInput = false;
            bulletRes = ResourceLoader.Load<PackedScene>("res://3_space_invaders/assets/bullet.tscn");
            lastFireTS = DateTime.Now;
        }

        public override void _Process(double delta)
        {
            int moveDir = 0;

            if (!disableInput)
            {
                if (Input.IsKeyPressed(inputMap["left"])) moveDir = -1;
                if (Input.IsKeyPressed(inputMap["right"])) moveDir = 1;
                if (Input.IsKeyPressed(inputMap["shoot"]) && (DateTime.Now - lastFireTS > FIRE_THRESH)) FireBullet(); 
            }

            MoveAndCollide(new Vector2(moveDir * moveSpeed, 0));    
        }

        [Signal] public delegate void OnPlayerFireEventHandler(); 

        private void FireBullet()
        {
            lastFireTS = DateTime.Now;
            BulletBehavior bullet = bulletRes.Instantiate<BulletBehavior>();
            bullet.Position = Position + Vector2.Up * 50;
            GetTree().Root.AddChild(bullet);
            EmitSignal(SignalName.OnPlayerFire); 
        }
    }
}

