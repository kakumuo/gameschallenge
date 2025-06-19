using Godot;
using System;

namespace BreakoutNamespace
{
    public partial class BallBehavior : CharacterBody2D
    {
        float baseSpeed = 400f;
        private Vector2 _ballVel;
        public Vector2 ballVel
        {
            get => _ballVel;
            set {
                _ballVel = value;
                EmitSignal(SignalName.OnVelocityChanged, value);
            }
        }
        const float COLLISION_THRESH = 10f;
        const float SPEED_UP_FACTOR = 1.05f; 

        public override void _Process(double delta)
        {
            var collision = MoveAndCollide(ballVel * ((float)delta));

            if (collision != null)
            {
                Vector2 colPoint = collision.GetPosition();

                var tempVel = ballVel;
                if (Math.Abs(colPoint.X - Position.X) > COLLISION_THRESH) tempVel.X *= -1;
                if (Math.Abs(colPoint.Y - Position.Y) > COLLISION_THRESH) tempVel.Y *= -1;

                if (collision.GetCollider() is PaddleController)
                {
                    PaddleController paddle = collision.GetCollider() as PaddleController;
                    CollisionShape2D collider = paddle.GetNode<CollisionShape2D>("collider");
                    float paddleWidth = (collider.Shape as RectangleShape2D).Size.X;

                    /*
                    divide paddlewidth by 1.25 to prevent ball from going horizontal when hitting edge
                    dividing by 2 forces ball to move horizontally outwards
                    */
                    float angle = (paddle.GlobalPosition.X - colPoint.X) / (paddleWidth / 1.25f);
                    angle = (float)((Math.PI * angle / 2) + Math.PI / 2);
                    
                    Vector2 targetDir = new Vector2(
                        (float)Math.Cos(angle),
                        -(float)Math.Sin(angle)
                    );
                    tempVel = targetDir.Normalized() * tempVel.DistanceTo(Vector2.Zero);
                }
                else if (collision.GetCollider() is BrickBehavior)
                {
                    EmitSignal(SignalName.OnBrickCollision, collision.GetCollider());
                    tempVel *= SPEED_UP_FACTOR;
                }

                ballVel = tempVel; 
            }
        }

        [Signal]
        public delegate void OnBrickCollisionEventHandler(BrickBehavior targetBrick); 

        [Signal]
        public delegate void OnVelocityChangedEventHandler(Vector2 newVel); 

        public void LaunchBall(Vector2 launchVec)
        {
            ballVel = launchVec.Normalized() * baseSpeed;
        }
    }
}
