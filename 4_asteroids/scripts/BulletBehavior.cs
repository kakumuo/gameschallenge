using System;
using Godot;

namespace AsteroidsNamespce
{
    public partial class BulletBehavior : Area2D
    {
        public Vector2 velocity;
        public int damage;

        public override void _Ready()
        {
            AreaEntered += (area) =>
            {
                if (area is AsteroidBehavior)
                {
                    (area as AsteroidBehavior).ApplyDamage(damage);
                    QueueFree(); 
                }
            };
        }

        public override void _Process(double delta)
        {
            Position += velocity * (float)delta;
        }
    }
} 