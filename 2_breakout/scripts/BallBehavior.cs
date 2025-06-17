using Godot;
using System;
using System.Drawing;


namespace BreakoutNamespace
{
    public partial class BallBehavior : CharacterBody2D
    {
        float baseSpeed = 400f; 
        Vector2 ballVel = new Vector2();
        const float COLLISION_THRESH = 10f;
        const float SPEED_UP_FACTOR = 1.05f; 

        public override void _Process(double delta)
        {
            var collision = MoveAndCollide(ballVel * ((float)delta));

            if (collision != null)
            {
                Vector2 colPoint = collision.GetPosition();

                if (Math.Abs(colPoint.X - Position.X) > COLLISION_THRESH) ballVel.X *= -1;
                if (Math.Abs(colPoint.Y - Position.Y) > COLLISION_THRESH) ballVel.Y *= -1;

                if (collision.GetCollider() is PaddleController)
                {
                    float ballSpeed = ballVel.DistanceTo(Vector2.Zero) * SPEED_UP_FACTOR;

                    PaddleController paddle = collision.GetCollider() as PaddleController;
                    CollisionShape2D collider = paddle.GetNode<CollisionShape2D>("collider");
                    float paddleWidth = (collider.Shape as RectangleShape2D).Size.X;

                    /*
                    divide paddlewidth by 1.25 to prevent ball from going horizontal when hitting edge
                    dividing by 2 forces ball to move horizontally outwards
                    */
                    float angle = (paddle.GlobalPosition.X - colPoint.X) / (paddleWidth / 1.25f);
                    angle = (float)((Math.PI * angle / 2) + Math.PI / 2); 
                    GD.Print("angle: ", angle);
                    Vector2 targetDir = new Vector2(
                        (float)Math.Cos(angle),
                        -(float)Math.Sin(angle)
                    ); 
                    ballVel = targetDir.Normalized() * ballSpeed; 
                }
            }
        }

        public void LaunchBall(Vector2 launchVec)
        {
            ballVel = launchVec.Normalized() * baseSpeed;
        }
    }
}
