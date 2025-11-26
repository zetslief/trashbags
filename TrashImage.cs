using Godot;
using System;

public record SizeSetup(Vector2 Scale, Vector2 TextureSize);
public record SelectSetup(Vector2 Scale, Vector2 Position, int ZIndex);

public partial class TrashImage : StaticBody2D
{
    private Sprite2D _sprite = default!;
    private Sprite2D _back = default!;
    private Vector2 _viewportSize;

    private Vector2 _initialScale;
    private bool _hovered = false;
    private Vector2 _initialPosition;
    private bool _selected = false;

    [Export]
    public Texture2D? Texture { get; set; }

    [Export]
    public Vector2 ShadowBorder { get; set; } = new(50, 50);

    [Export]
    public Color ShadowColor { get; set; } = Colors.White;

    [Export]
    public Texture2D? Back { get; set; }

    public bool IsSelected => _selected;
    public bool IsHovered => _hovered;
    public Vector2 InitialScale => _initialScale;
    public bool IsFlipped { get; private set; } = false;

    public override void _Ready()
    {
        if (Texture is null) throw new InvalidOperationException();
        _viewportSize = GetViewportRect().Size;
        _sprite = GetNode<Sprite2D>("Sprite");
        _sprite.Texture = Texture;
        var collider = GetNode<CollisionShape2D>("Collider");
        var colliderSize = _sprite.GetRect().Size;
        collider.Shape = new RectangleShape2D() { Size = colliderSize };
        collider.Position = colliderSize / 2;
        var shadow = GetNode<MeshInstance2D>("Shadow");
        var shadowSize = colliderSize + ShadowBorder;
        shadow.Modulate = ShadowColor;
        shadow.Scale = shadowSize;
        shadow.Position = shadowSize / 2f - ShadowBorder / 2;
        _back = GetNode<Sprite2D>("Back");
        _back.Position = colliderSize / 2;
        if (Back is not null) _back.Texture = Back;
    }

    public override void _MouseEnter() => _hovered = true;
    public override void _MouseExit() => _hovered = false;

    public override void _Process(double delta)
    {
    }

    public static SelectSetup SetupSelect(TrashImage image)
    {
        image._selected = !image._selected;
        if (image._selected)
        {
            image._initialPosition = image.Position;
            var currentScale = image.Scale;
            var currentSpriteSize = image._sprite.GetRect().Size * currentScale;
            var targetSpriteWidth = image._viewportSize.X * 0.4f;
            var multiplier = targetSpriteWidth / currentSpriteSize.X;
            var targetPosition = new Vector2((image._viewportSize.X - targetSpriteWidth) / 2, image._viewportSize.Y * 0.009f);
            return new(image.Scale * multiplier, targetPosition, 1000);
        }
        else
        {
            return new(image._initialScale, image._initialPosition, 0);
        }
    }

    public static void SetupFlip(TrashImage image)
    {
        image.IsFlipped = true;
        image._back.Texture = null;
        GD.Print("Flipped");
    }

    public static SizeSetup SetupSize(TrashImage trashImage, float height)
    {
        var sprite = trashImage._sprite;
        var currentScale = trashImage.Scale;
        var spriteSize = sprite.GetRect().Size;
        var currentSpriteSize = spriteSize * currentScale;
        var resultScale = IsVertical(currentSpriteSize)
            ? CalculateScaleForHeight(currentScale, currentSpriteSize, height)
            : CalculateScaleForWidth(currentScale, currentSpriteSize, height);
        trashImage._initialScale = resultScale;
        GD.Print(spriteSize * resultScale);
        return new(resultScale, spriteSize);
    }

    private static Vector2 CalculateScaleForHeight(Vector2 currentScale, Vector2 currentSize, float requiredHeight)
    {
        var multiplier = requiredHeight / currentSize.Y;
        return currentScale * multiplier;
    }

    private static Vector2 CalculateScaleForWidth(Vector2 currentScale, Vector2 currentSize, float requiredWidth)
    {
        var multiplier = requiredWidth / currentSize.X;
        return currentScale * multiplier;
    }

    private static bool IsVertical(Vector2 size) => size.X < size.Y;
}