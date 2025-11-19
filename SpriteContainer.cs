using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SpriteContainer : Node2D
{
    private Vector2 _viewportSize;

    [Export]
    public PackedScene TrashImageScene { get; set; } = default!;

    [Export]
    public Marker2D PositionMarker { get ;set; } = default!;

    [Export]
    public float ImageClearOffset { get; set; } = 15;

    [Export]
    public double ImageClearDuration { get; set; } = 0.7;

    [Export]
    public double ImageClearDelay { get; set; } = 0.1;

    [Export]
    public Tween.EaseType Ease { get; set; } = Tween.EaseType.InOut;

    [Export]
    public Tween.TransitionType TransitionType { get; set; } = Tween.TransitionType.Linear;

    public override void _Ready()
    {
        if (TrashImageScene is null) throw new InvalidOperationException($"Trash image scene is not set.");
        if (PositionMarker is null) throw new InvalidOperationException($"PositionMarker is not set.");
        _viewportSize = GetViewportRect().Size;
        var textures = Enumerable.Range(1, 14).Select(i => (i, GD.Load<Texture2D>($"res://images/trash_{i}.png")));
        foreach (var (index, texture) in textures)
        {
            var node = TrashImageScene.Instantiate<TrashImage>();
            node.Name = $"trash_{index}";
            node.Texture = texture;
            AddChild(node);
        }
        DirectShuffle(this);
        Shuffle(this);
    }

    public static void Shuffle(SpriteContainer container)
    {
        foreach (var (position, child) in GetShuffleSetup(container))
        {
            var tween = container.CreateTween();
            tween.TweenProperty(child, Property.Position, position, 1.5)
                .SetEase(container.Ease)
                .SetTrans(container.TransitionType);
        }
    }

    public static void Clear(SpriteContainer container)
    {
        var targetPosition = container.PositionMarker.Position;
        int index = 0;
        foreach (var child in container.GetChildren())
        {
            if (child is not TrashImage trashImage) continue;
            var tween = container.CreateTween();
            var offset = Vec2.Random(-container.ImageClearOffset, container.ImageClearOffset);
            tween.TweenInterval(container.ImageClearDelay * index);
            tween.TweenProperty(trashImage, Property.Position, targetPosition + offset, container.ImageClearDuration)
               .SetTrans(Tween.TransitionType.Cubic);
            index ++;
        }
    }

    public static void Random(SpriteContainer container)
    {
        foreach (var child in container.GetChildren())
        {
            if (child is TrashImage { IsSelected: true } selectedImage)
                TrashImage.Select(selectedImage);
        }
        var randomNodeIndex = container.GetChildCount();
        if (randomNodeIndex < 0) throw new InvalidOperationException("SpriteContainer contains no nodes.");
        randomNodeIndex = System.Random.Shared.Next(randomNodeIndex);
        var imageToSelect = container.GetChild<TrashImage>(randomNodeIndex);
        TrashImage.Select(imageToSelect);
    }

    private static void DirectShuffle(SpriteContainer container)
    {
        foreach (var (position, child) in GetShuffleSetup(container))
        {
            child.Position = position;
        }
    }

    private static IEnumerable<(Vector2 Position, TrashImage child)> GetShuffleSetup(SpriteContainer container)
    {
        foreach (var child in container.GetChildren())
        {
            if (child is TrashImage sprite)
            {
                var resultSize = TrashImage.SetSize(sprite, 204);
                var standardOffset = new Vector2(container._viewportSize.X * 0.01f, container._viewportSize.Y * 0.05f);
                var targetPosition = CalculateRandomPosition(container._viewportSize, standardOffset, resultSize);
                yield return (targetPosition, sprite);
            }
        }
    }

    private static Vector2 CalculateRandomPosition(Vector2 viewportSize, Vector2 defaultOffset, Vector2 rightBottomOffset)
    {
        return defaultOffset + Vec2.Random() * (viewportSize - defaultOffset * 2f - rightBottomOffset);
    }
}
