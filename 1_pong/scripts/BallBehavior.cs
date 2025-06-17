using Godot;
using System;

namespace PongNamespace
{

    public partial class BallBehavior : CharacterBody2D
    {

        [Export]
        public int ballSpeed = 500;
        [Export]
        public float incFactor = 1.05f;
        public Vector2 ballVel = Vector2.Zero;

        public override void _Ready()
        {
            LaunchBall(ballSpeed);
        }

        const float COLLISION_THRESH = 10;
        public override void _Process(double delta)
        {
            var collisionInfo = MoveAndCollide(this.ballVel * ((float)delta));
            if (collisionInfo != null)
            {
                CollisionObject2D collider = collisionInfo.GetCollider() as CollisionObject2D;
                Vector2 collisionPoint = collisionInfo.GetPosition();

                if (Math.Abs(collisionPoint.X - Position.X) > COLLISION_THRESH) ballVel.X *= -1;
                if (Math.Abs(collisionPoint.Y - Position.Y) > COLLISION_THRESH) ballVel.Y *= -1;

                if (collider.Name == "p1" || collider.Name == "p2")
                    ballVel *= incFactor;
            }
        }

        public void LaunchBall(float launchSpeed = 0, Vector2 dir = new Vector2())
        {
            float targetSpeed = launchSpeed == 0 ? this.ballSpeed : launchSpeed;
            Vector2 targetDir = new Vector2((GD.Randf() - .5f) * 2, GD.Randf() - .5f);
            if (dir != Vector2.Zero)
            {
                ballVel = dir * targetSpeed;
            }
            else
            {
                ballVel = targetDir.Normalized() * targetSpeed;
            }

        }
    }

}