using Godot;
using System;
using System.Collections.Generic;

namespace SpaceInvadersNS
{
    public partial class BulletBehavior : StaticBody2D
    {
        Vector2 bulletVel = Vector2.Zero;
        public float bulletSpeed = 200f; 

        public enum BulletOwnerType
        {
            PLAYER, ENEMY
        }
        public BulletOwnerType bulletOwner = BulletOwnerType.PLAYER;


        Sprite2D sprite; 

        public override void _Ready()
        {
            bulletVel = (bulletOwner == BulletOwnerType.PLAYER ? Vector2.Up : Vector2.Down) * bulletSpeed;
            sprite = GetNode<Sprite2D>("sprite");
            string targetPath = "";
            if (bulletOwner == BulletOwnerType.ENEMY)
            {
                targetPath = "res://3_space_invaders/sprites/enemy_bullet.png";
                SetCollisionMaskValue(1, true);
            }
            else if (bulletOwner == BulletOwnerType.PLAYER)
            {
                targetPath = "res://3_space_invaders/sprites/bullet_sprite.png";
                SetCollisionMaskValue(5, true);
            }

            Image img = Image.LoadFromFile(targetPath);  //ResourceLoader.Load<Image>(targetPath); 
            sprite.Texture = ImageTexture.CreateFromImage(img);
        }

        public override void _Process(double delta)
        {
            var col = MoveAndCollide(bulletVel * ((float)delta));
            if (col != null)
            {
                if (col.GetCollider() is EnemyBehavior && bulletOwner == BulletOwnerType.PLAYER)
                {
                    (col.GetCollider() as EnemyBehavior).Kill();
                    Free();
                }
                else if (col.GetCollider() is PlayerController && bulletOwner == BulletOwnerType.ENEMY)
                {
                    (col.GetCollider() as PlayerController).ApplyDamage();
                    Free();
                }
                else if (col.GetCollider() is BulletBehavior && bulletOwner == BulletOwnerType.PLAYER && (col.GetCollider() as BulletBehavior).bulletOwner == BulletOwnerType.ENEMY)
                {
                    Free();
                    col.GetCollider().Free(); 
                }
            }
        }
    }
}