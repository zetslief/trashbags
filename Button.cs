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
                var resultSize = TrashImage.SetSize(sprite, 204);
                var standardOffset = new Vector2(_viewportSize.X * 0.01f, _viewportSize.Y * 0.05f);
                var targetPosition = CalculateRandomPosition(_viewportSize, standardOffset, resultSize);
                var tween = CreateTween();
                tween.TweenProperty(child, (string)Node2D.PropertyName.Position, targetPosition, 3)
                    .SetEase(Tween.EaseType.In)
                    .SetTrans(Tween.TransitionType.Linear);
            }
        }
    }

    private static Vector2 CalculateRandomPosition(Vector2 viewportSize, Vector2 defaultOffset, Vector2 rightBottomOffset)
    {
        var random = new Vector2(Random.Shared.NextSingle(), Random.Shared.NextSingle());
        return defaultOffset + random * (viewportSize - defaultOffset * 2f - rightBottomOffset);
    }
}
