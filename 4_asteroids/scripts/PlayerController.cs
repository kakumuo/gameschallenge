using Godot;
using System;
using System.Collections.Generic;

namespace AsteroidsNamespce
{
    public partial class PlayerController : CharacterBody2D
    {

        [Export] public float turnSpeed = 20f;
        [Export] public float moveSpeed = 5f;
        [Export] public float fireRate = 10f;
        private Vector2 targetVel;
        private float moveDecel = 2f;
        private float turnDecel = 5f;
        private float facingAngle;
        private float targetAngle; 
        enum IT { UP, DOWN, TURN_LEFT, TURN_RIGHT, STRAFE, FIRE }
        Dictionary<IT, Key> inputMap;

        Sprite2D sprite;

        public override void _Ready()
        {
            targetVel = Vector2.Zero;
            facingAngle = 90;
            targetAngle = facingAngle;

            inputMap = new Dictionary<IT, Key>
            {
                {IT.UP, Key.W}, {IT.DOWN, Key.S}, {IT.TURN_LEFT, Key.A}, {IT.TURN_RIGHT, Key.D},
                {IT.STRAFE, Key.Shift}, {IT.FIRE, Key.Space}
            };

            sprite = GetNode<Sprite2D>("sprite"); 
        }

        public override void _Process(double delta)
        {
            Vector2 moveVec = Vector2.Zero;

            if (Input.IsKeyPressed(inputMap[IT.UP])) moveVec += getFacingDir();
            if (Input.IsKeyPressed(inputMap[IT.DOWN])) moveVec -= getFacingDir();

            if (Input.IsKeyPressed(inputMap[IT.STRAFE]))
            {
                if (Input.IsKeyPressed(inputMap[IT.TURN_LEFT])) moveVec += getFacingDir(-90);
                if (Input.IsKeyPressed(inputMap[IT.TURN_RIGHT])) moveVec += getFacingDir(90);
            }
            else
            {
                if (Input.IsKeyPressed(inputMap[IT.TURN_LEFT])) facingAngle -= 1;
                if (Input.IsKeyPressed(inputMap[IT.TURN_RIGHT])) facingAngle += 1;
            }

            targetVel = targetVel.Lerp(moveVec, moveDecel * ((float)delta));
            targetAngle = Mathf.Lerp(targetAngle, facingAngle, turnDecel * ((float)delta));
            RotationDegrees = targetAngle - 90; 
            var col = MoveAndCollide(targetVel * moveSpeed);
        }

        public Vector2 getFacingDir(float angleAdjust = 0)
        {
            float turnPercent = (targetAngle + angleAdjust) / 360.0f;
            return new Vector2(-(float)Math.Cos(turnPercent * Math.PI * 2), -(float)Math.Sin(turnPercent * Math.PI * 2));
        }

    }
}

