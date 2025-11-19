using Godot;
using System;

public partial class TrashImage : StaticBody2D
{
    private Sprite2D _sprite = default!;
    private Vector2 _viewportSize;

    private Vector2 _initialScale;
    private bool _hovered = false;
    private Vector2 _initialPosition;
    private bool _selected = false;

    [Export]
    public Texture2D? Texture { get; set; }

    public bool IsSelected => _selected;
    public bool IsHovered => _hovered;

    public override void _Ready()
    {
        if (Texture is null) throw new InvalidOperationException();
        _viewportSize = GetViewportRect().Size;
        _sprite = GetNode<Sprite2D>("Sprite");
        _sprite.Texture = Texture;
        var collider = GetNode<CollisionShape2D>("Collider");
        var colliderSize = _sprite.GetRect().Size;
        collider.Shape = new RectangleShape2D() { Size = colliderSize };
        collider.Position = colliderSize / 2f;
    }

    public override void _MouseEnter() =>_hovered = true;
    public override void _MouseExit() => _hovered = false;

    public override void _Input(InputEvent @event)
    {
        if (!_hovered) return;
        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && mb.IsPressed())
        {
            Select(this);
        }
    }

    public override void _Process(double delta)
    {
    }

    public static void Select(TrashImage image)
    {
        image._selected = !image._selected;
        var tween = image.CreateTween().SetParallel(true);
        if (image._selected)
        {
            image._initialPosition = image.Position;
            var currentScale = image.Scale;
            var currentSpriteSize = image._sprite.GetRect().Size * currentScale;
            var targetSpriteWidth = image._viewportSize.X * 0.4f;
            var multiplier = targetSpriteWidth / currentSpriteSize.X;
            var targetPosition = new Vector2((image._viewportSize.X - targetSpriteWidth) / 2, image._viewportSize.Y * 0.009f);
            tween.TweenProperty(image, (string)Node2D.PropertyName.Scale, image.Scale * multiplier, 1);
            tween.TweenProperty(image, (string)Node2D.PropertyName.Position, targetPosition, 1);
            tween.TweenProperty(image, (string)CanvasItem.PropertyName.ZIndex, 1000, 1);
        }
        else
        {
            tween.TweenProperty(image, (string)Node2D.PropertyName.Scale, image._initialScale, 1);
            tween.TweenProperty(image, (string)Node2D.PropertyName.Position, image._initialPosition, 1);
            tween.TweenProperty(image, (string)CanvasItem.PropertyName.ZIndex, 0, 1);
        }
    }

    public static Vector2 SetSize(TrashImage trashImage, float height)
    {
        var sprite = trashImage._sprite;
        var currentScale = trashImage.Scale;
        var spriteSize = sprite.GetRect().Size;
        var currentSpriteSize = spriteSize * currentScale;
        var resultScale = CalculateScale(currentScale, currentSpriteSize, height);
        trashImage.Scale = resultScale;
        trashImage._initialScale = trashImage.Scale;
        return spriteSize * resultScale;
    }

    private static Vector2 CalculateScale(Vector2 currentScale, Vector2 currentSize, float requiredHeight)
    {
        var multiplier = requiredHeight / currentSize.Y;
        return currentScale * multiplier;
    }
}