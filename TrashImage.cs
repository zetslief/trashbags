using Godot;
using System;

public partial class TrashImage : StaticBody2D
{
    private Vector2 initialScale;

    [Export]
    private Texture2D? Texture { get; set; }

    public override void _Ready()
    {
        if (Texture is null) throw new InvalidOperationException();
        var sprite = GetNode<Sprite2D>("Sprite");
        sprite.Texture = Texture;
        var collider = GetNode<CollisionShape2D>("Collider");
        collider.Shape = new RectangleShape2D()
        {
            Size = sprite.GetRect().Size
        };
    }

    public override void _MouseEnter()
    {
        initialScale = Scale;
        Scale = initialScale * 1.2f;
    }

    public override void _MouseExit()
    {
        Scale = initialScale;
    }

    public static void SetSize(TrashImage trashImage, float height)
    {
        var sprite = trashImage.GetNode<Sprite2D>("Sprite");
        var collider = trashImage.GetNode<CollisionShape2D>("Collider");
        var currentScale = sprite.Scale;
        var currentSpriteSize = sprite.GetRect().Size * currentScale;
        var resultScale = CalculateScale(currentScale, currentSpriteSize, height);
        sprite.Scale = resultScale;
        collider.Scale = resultScale;
    }

    private static Vector2 CalculateScale(Vector2 currentScale, Vector2 currentSize, float requiredHeight)
    {
        var multiplier = requiredHeight / currentSize.Y;
        return currentScale * multiplier;
    }
}