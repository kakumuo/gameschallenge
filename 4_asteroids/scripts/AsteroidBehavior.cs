using Godot;
using System;
using System.Collections.Generic;

namespace AsteroidsNamespce
{
    public partial class AsteroidBehavior : Area2D
    {
        public float moveSpeed = 5f;
        public int asteroidHealth = 0;
        public int asteroidType = 0;
        private Vector2 moveDir;

        private Sprite2D sprite;
        private CollisionShape2D shape;

        public override void _Ready()
        {
            sprite = GetNode<Sprite2D>("sprite");
            shape = GetNode<CollisionShape2D>("collider");
        }

        public void spawn(int asteroidType, Vector2 spawnLocation, Vector2 moveDir)
        {
            this.Position = spawnLocation;
            this.asteroidType = asteroidType;
            this.moveDir = moveDir;

            string targetSpritePath = "";
            if (asteroidType == 0)
            {
                asteroidHealth = 20;
                targetSpritePath = "res://4_asteroids/sprites/asteroid_01.png";
                (shape.Shape as RectangleShape2D).Size = new Vector2(20, 20);
                this.moveDir *= 3;
            }
            else if (asteroidType == 1)
            {
                asteroidHealth = 50;
                targetSpritePath = "res://4_asteroids/sprites/asteroid_02.png";
                (shape.Shape as RectangleShape2D).Size = new Vector2(40, 40);
                this.moveDir *= 2;
            }
            else if (asteroidType == 2)
            {
                asteroidHealth = 100;
                targetSpritePath = "res://4_asteroids/sprites/asteroid_03.png";
                (shape.Shape as RectangleShape2D).Size = new Vector2(70, 70);
                this.moveDir *= 1;
            }

            sprite.Texture = ResourceLoader.Load<Texture2D>(targetSpritePath);
        }

        public override void _Process(double delta)
        {
            Position += moveDir * (float)delta * 100;
        }


        [Signal] public delegate void OnAsteroidDestroyedEventHandler(int asteroidType);  
        public void ApplyDamage(int damageAmount)
        {
            if (damageAmount > asteroidHealth)
            {
                EmitSignal(SignalName.OnAsteroidDestroyed, asteroidType); 
                QueueFree();
            }
            else
            {
                asteroidHealth -= damageAmount;
            }
        }
            
    }
}

