using Godot;
using System;
using System.Collections.Generic;

namespace SpaceInvadersNS
{
    public partial class BulletBehavior : StaticBody2D
    {

        Vector2 bulletVel = Vector2.Zero;

        public override void _Ready()
        {
            bulletVel = Vector2.Up * 200f;
        }

        public override void _Process(double delta)
        {
            var col = MoveAndCollide(bulletVel * ((float)delta));
            if (col != null)
            {
                if (col.GetCollider() is EnemyBehavior)
                {
                    (col.GetCollider() as EnemyBehavior).Kill(); 
                }
                Free();
            }
        }
    }
}