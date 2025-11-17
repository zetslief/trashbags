using Godot;
using System;
using System.Linq;

public partial class SpriteContainer : Node2D
{
    private Vector2 _viewportSize;

    [Export]
    public PackedScene TrashImageScene { get; set; } = default!;

    public override void _Ready()
    {
        if (TrashImageScene is null) throw new InvalidOperationException($"Trash image scene is not set.");
        _viewportSize = GetViewportRect().Size;
        var textures = Enumerable.Range(1, 14).Select(i => (i, GD.Load<Texture2D>($"res://images/trash_{i}.png")));
        foreach (var (index, texture) in textures)
        {
            var node = TrashImageScene.Instantiate<TrashImage>();
            node.Name = $"trash_{index}";
            node.Texture = texture;
            AddChild(node);
        }
        Shuffle(this);
    }

    public static void Shuffle(SpriteContainer container)
    {
        foreach (var child in container.GetChildren())
        {
            if (child is TrashImage sprite)
            {
                var resultSize = TrashImage.SetSize(sprite, 204);
                var standardOffset = new Vector2(container._viewportSize.X * 0.01f, container._viewportSize.Y * 0.05f);
                var targetPosition = CalculateRandomPosition(container._viewportSize, standardOffset, resultSize);
                var tween = container.CreateTween();
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
