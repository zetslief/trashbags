using Godot;
using System;

public partial class Button : Godot.Button
{
    private Vector2 _viewportSize;

    public override void _Ready()
    {
        _viewportSize = GetViewportRect().Size;
        _Pressed();
    }

    public override void _Pressed()
    {
        var spriteContainer = GetNode("/root/Main/SpriteContainer")
            ?? throw new InvalidOperationException($"Failed to find sprite container node.");;
        foreach (var child in spriteContainer.GetChildren())
        {
            if (child is Sprite2D sprite)
            {
                var currentScale = sprite.Scale;
                var currentSpriteSize = sprite.GetRect().Size * currentScale;
                var (resultScale, resultSize) = CalculateScale(currentScale, currentSpriteSize, 204);
                sprite.Scale = resultScale;
                var targetPosition = CalculateRandomPosition(_viewportSize, resultSize);
                var tween = CreateTween();
                tween.TweenProperty(child, "position", targetPosition, 3)
                    .SetEase(Tween.EaseType.In)
                    .SetTrans(Tween.TransitionType.Linear);
            }
        }
    }

    private static (Vector2 Scale, Vector2 Size) CalculateScale(Vector2 currentScale, Vector2 currentSize, float requiredHeight)
    {
        var multiplier = requiredHeight / currentSize.Y;
        return (currentScale * multiplier, currentSize * multiplier);
    }

    private static Vector2 CalculateRandomPosition(Vector2 viewportSize, Vector2 imageSize)
    {
        var x = (imageSize.X / 2) + Random.Shared.NextSingle() * (viewportSize.X - imageSize.X);
        var y = (imageSize.Y / 2) + Random.Shared.NextSingle() * (viewportSize.Y - imageSize.Y);
        return new(x, y);
    }
}
