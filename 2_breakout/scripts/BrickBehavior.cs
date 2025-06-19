using Godot;
using System;

public partial class BrickBehavior : StaticBody2D
{
    [Export]
    Vector2 size;
    CollisionShape2D collider;
    Sprite2D sprite;
    public override void _Ready()
    {
        collider = GetNode<CollisionShape2D>("collider");
        sprite = GetNode<Sprite2D>("sprite");
    }

    public void setSize(float width, float height)
    {
        size = new Vector2(width, height);
        (collider.Shape as RectangleShape2D).Size = size;

        Vector2 textureSize = sprite.Texture.GetSize();
        Vector2 scale = new Vector2(width / textureSize.X, height / textureSize.Y);

        sprite.Scale = scale;
    }
}
 