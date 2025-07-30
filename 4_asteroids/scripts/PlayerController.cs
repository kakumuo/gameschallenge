using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Timers;

namespace AsteroidsNamespce
{
    public partial class PlayerController : Area2D
    {

        // MOVEMENT VARIABLES
        [Export] public float turnSpeed = 20f;
        [Export] public float moveSpeed = 10f;
        [Export] public float maxSpeed = 8f;
        private float moveDecel = 1f;
        private float turnDecel = 10f;
        private float facingAngle;
        private float targetAngle;
        private Vector2 targetVel;

        // FIRING SYSTEM
        private int fireRateMS = 300;
        private int reloadSpeedMS = 1500;
        private int maxAmmo = 30;
        private int weaponDamage = 30;
        public int curAmmo = 0; 
        private DateTime lastFireTS;
        private TimeSpan fireRateSpan;
        public TimerPlus reloadTimer;

        // HEALTH SYSTEM
        private int maxHealth = 3;
        public int remHealth = 0;
        public DateTime lastDamageTS;
        public TimerPlus damageInvulTimer; 

        // INPUT SYSTEM
        private enum IT { UP, DOWN, LEFT, RIGHT, PRI_FIRE }
        private Dictionary<IT, Key> kbdInputMap;
        private Dictionary<IT, MouseButton> mouseInputMap;


        Sprite2D sprite;
        CollisionShape2D collider; 
        PackedScene bulletRes;

        public override void _Ready()
        {
            sprite = GetNode<Sprite2D>("sprite");
            collider = GetNode<CollisionShape2D>("collider");

            bulletRes = ResourceLoader.Load<PackedScene>("res://4_asteroids/assets/bullet.tscn");

            targetVel = Vector2.Zero;
            facingAngle = 0;
            targetAngle = facingAngle;

            kbdInputMap = new Dictionary<IT, Key>
            {
                {IT.UP, Key.W}, {IT.DOWN, Key.S}, {IT.LEFT, Key.A}, {IT.RIGHT, Key.D}
            };

            mouseInputMap = new Dictionary<IT, MouseButton>
            {
                {IT.PRI_FIRE, MouseButton.Left}
            };


            fireRateSpan = new TimeSpan(fireRateMS * 10_000);
            lastFireTS = DateTime.Now;

            // weapon
            curAmmo = maxAmmo;
            reloadTimer = new TimerPlus(reloadSpeedMS, () => { curAmmo = maxAmmo; });

            // health
            lastDamageTS = DateTime.Now;
            damageInvulTimer = new TimerPlus(1000, () => { sprite.SetDeferred("modulate", Colors.White); });
            remHealth = maxHealth; 
            AreaEntered += (area) =>
            {
                if (damageInvulTimer.isEnabled) return; 
                if (area is AsteroidBehavior)
                    ApplyDamage(); 
            };        
        }

        public void ResetPlayer()
        {
            targetVel = Vector2.Zero; 
            Position = Vector2.Zero; 
            curAmmo = maxAmmo;
            remHealth = maxHealth; 
        }

        public override void _Process(double delta)
        {
            Vector2 moveVec = Vector2.Zero;
            float targetMoveSpeed = moveSpeed;

            if (Input.IsKeyPressed(kbdInputMap[IT.UP])) moveVec += Vector2.Up;
            if (Input.IsKeyPressed(kbdInputMap[IT.DOWN])) moveVec -= Vector2.Up;
            if (Input.IsKeyPressed(kbdInputMap[IT.LEFT])) moveVec += Vector2.Left;
            if (Input.IsKeyPressed(kbdInputMap[IT.RIGHT])) moveVec -= Vector2.Left;

            var mousePos = GetTree().Root.GetCamera2D().GetGlobalMousePosition();
            facingAngle = (float)(mousePos.AngleToPoint(Position) * 360 / (2 * Math.PI));

            if (Input.IsMouseButtonPressed(mouseInputMap[IT.PRI_FIRE]) && !reloadTimer.isEnabled)
            {
                DoFire();
            }

            targetVel = targetVel.Lerp(moveVec, moveDecel * ((float)delta));
            targetAngle = facingAngle;
            RotationDegrees = targetAngle;

            Vector2 targetMoveVec = targetVel * targetMoveSpeed;
            Position += targetMoveVec * (float)delta * 100;
        }

        private void DoFire()
        {
            if (curAmmo == 0)
            {
                reloadTimer.Start(); 
            }
            if (DateTime.Now - lastFireTS < fireRateSpan) return;

            BulletBehavior bullet = bulletRes.Instantiate<BulletBehavior>();
            bullet.damage = weaponDamage; 
            bullet.velocity = GetFacingDir() * 1200;
            bullet.GlobalPosition = GlobalPosition + GetFacingDir() * 20;
            curAmmo--;

            GetTree().Root.AddChild(bullet);
            lastFireTS = DateTime.Now; 
        }

        [Signal] public delegate void OnPlayerKilledEventHandler();

        public void ApplyDamage()
        {
            remHealth -= 1;
            damageInvulTimer.Start();
            sprite.SetDeferred("modulate", Colors.Gray);

            if (remHealth == 0)
            {
                EmitSignal(SignalName.OnPlayerKilled); 
            }
        }

        public Vector2 GetFacingDir(float angleAdjust = 0)
        {
            float turnPercent = (targetAngle + angleAdjust) / 360.0f;
            return new Vector2(-(float)Math.Cos(turnPercent * Math.PI * 2), -(float)Math.Sin(turnPercent * Math.PI * 2));
        }
    }
}

