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
            if (child is TrashImage sprite)
            {
                TrashImage.SetSize(sprite, 204);
                var targetPosition = CalculateRandomPosition(_viewportSize, _viewportSize * 0.1f);
                var tween = CreateTween();
                tween.TweenProperty(child, (string)Node2D.PropertyName.Position, targetPosition, 3)
                    .SetEase(Tween.EaseType.In)
                    .SetTrans(Tween.TransitionType.Linear);
            }
        }
    }

    private static Vector2 CalculateRandomPosition(Vector2 viewportSize, Vector2 offset)
    {
        var x = (offset.X / 2) + Random.Shared.NextSingle() * (viewportSize.X - offset.X);
        var y = (offset.Y / 2) + Random.Shared.NextSingle() * (viewportSize.Y - offset.Y);
        return new(x, y);
    }
}
