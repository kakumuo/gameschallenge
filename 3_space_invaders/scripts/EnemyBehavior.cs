using Godot;
using SpaceInvadersNS;
using System;

public partial class EnemyBehavior : StaticBody2D
{

    [Export(PropertyHint.Range, "0,2,")] int enemyLevel = 0;
    private Sprite2D sprite;
    private DateTime lastFireTS;
    private TimeSpan fireRateThresh;
    private PackedScene bulletRes; 

    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("sprite");

        string[] spriteImageList = {
            "res://3_space_invaders/sprites/alien_sprite_001.png",
            "res://3_space_invaders/sprites/alien_sprite_002.png",
            "res://3_space_invaders/sprites/alien_sprite_003.png",
        };
        var imageRes = Image.LoadFromFile(spriteImageList[enemyLevel]); //ResourceLoader.Load<Image>(spriteImageList[enemyLevel]); 
        sprite.Texture = ImageTexture.CreateFromImage(imageRes);
        bulletRes = ResourceLoader.Load<PackedScene>("res://3_space_invaders/assets/bullet.tscn");


        lastFireTS = DateTime.Now;
        fireRateThresh = new TimeSpan(0, 0, (int)(GD.Randf() * 10));
    }


    public override void _Process(double delta)
    {
        if (DateTime.Now - lastFireTS > fireRateThresh)
        {
            DoFire(); 
        }
    }

    public void DoFire()
    {
        BulletBehavior bulletObj = bulletRes.Instantiate<BulletBehavior>(); 
        bulletObj.bulletOwner = BulletBehavior.BulletOwnerType.ENEMY;
        bulletObj.Position = GlobalPosition + (Vector2.Down * 2);
        bulletObj.bulletSpeed = 100f; 
        GetTree().Root.AddChild(bulletObj);  
    
        lastFireTS = DateTime.Now;
        fireRateThresh = new TimeSpan(0, 0, (int)(GD.Randf() * 10));
    }

    public void Kill()
    {
        EmitSignal(SignalName.OnEnemyKilled);
        Free();
    }

    [Signal] public delegate void OnEnemyKilledEventHandler(); 
}
