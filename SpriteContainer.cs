using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SpriteContainer : Node2D
{
    private const int _minZ = 0;
    private const int _maxZ = 100;
    private Vector2 _viewportSize;

    [Export]
    public PackedScene TrashImageScene { get; set; } = default!;

    [Export]
    public Marker2D PositionMarker { get; set; } = default!;

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

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventMouseButton mb || mb.ButtonIndex == MouseButton.Left && mb.IsPressed())
            return;

        foreach (var child in GetChildren())
        {
            if (child is TrashImage { IsSelected: true, IsHovered: false } selectedImage)
                RunSelect(selectedImage, TrashImage.SetupSelect(selectedImage));
        }

        foreach (var child in GetChildren())
        {
            if (child is TrashImage { IsHovered: true } hoveredImage)
            {
                if (hoveredImage.IsFlipped) RunSelect(hoveredImage, TrashImage.SetupSelect(hoveredImage));
                else TrashImage.SetupFlip(hoveredImage);
                break;
            }
        }
    }

    public static void Shuffle(SpriteContainer container)
    {
        foreach (var (position, child) in GetShuffleSetup(container))
        {
            if (child is not TrashImage trashImage) continue;
            var tween = container.CreateTween();
            tween.TweenProperty(trashImage, Property.Position, position, 1.5)
                .SetEase(container.Ease)
                .SetTrans(container.TransitionType);
        }
    }

    public static void Clear(SpriteContainer container)
    {
        var targetPosition = container.PositionMarker.Position;
        int index = 0;
        var children = container.GetChildren();
        foreach (var child in children)
        {
            if (child is not TrashImage trashImage) continue;
            var zIndex = System.Random.Shared.Next(_minZ, _maxZ);
            var offset = Vec2.Random(-container.ImageClearOffset, container.ImageClearOffset);
            var scale = trashImage.Scale;
            if (trashImage.IsSelected)
            {
                scale = TrashImage.SetupSelect(trashImage).Scale;
            }
            var tween = container.CreateTween();
            tween.TweenInterval(container.ImageClearDelay * index);
            tween.TweenProperty(trashImage, Property.Position, targetPosition + offset, container.ImageClearDuration)
                .SetTrans(Tween.TransitionType.Cubic);
            if (scale != trashImage.Scale)
                tween.Parallel().TweenProperty(trashImage, Property.Scale, scale, container.ImageClearDuration);
            tween.Parallel().TweenProperty(trashImage, Property.ZIndex, zIndex, container.ImageClearDuration / 2);
            index++;
        }
    }

    public static void Random(SpriteContainer container)
    {
        foreach (var child in container.GetChildren())
        {
            if (child is TrashImage { IsSelected: true } selectedImage)
                RunSelect(selectedImage, TrashImage.SetupSelect(selectedImage));
        }
        var randomNodeIndex = container.GetChildCount();
        if (randomNodeIndex < 0) throw new InvalidOperationException("SpriteContainer contains no nodes.");
        randomNodeIndex = System.Random.Shared.Next(randomNodeIndex);
        var imageToSelect = container.GetChild<TrashImage>(randomNodeIndex);
        RunSelect(imageToSelect, TrashImage.SetupSelect(imageToSelect));
    }

    private static void DirectShuffle(SpriteContainer container)
    {
        foreach (var (position, child) in GetShuffleSetup(container))
        {
            child.Position = position;
        }
    }

    private static void RunSelect(TrashImage image, SelectSetup setup)
    {
        var tween = image.CreateTween().SetParallel(true);
        tween.TweenProperty(image, (string)Node2D.PropertyName.Scale, setup.Scale, 1);
        tween.TweenProperty(image, (string)Node2D.PropertyName.Position, setup.Position, 1);
        tween.TweenProperty(image, (string)CanvasItem.PropertyName.ZIndex, setup.ZIndex, 1);
    }

    private static IEnumerable<(Vector2 Position, TrashImage child)> GetShuffleSetup(SpriteContainer container)
    {
        foreach (var child in container.GetChildren())
        {
            if (child is TrashImage sprite && !sprite.IsSelected)
            {
                var sizeSetup = TrashImage.SetupSize(sprite, 204);
                var standardOffset = new Vector2(container._viewportSize.X * 0.01f, container._viewportSize.Y * 0.05f);
                var resultSize = sizeSetup.Scale * sizeSetup.TextureSize;
                var targetPosition = CalculateRandomPosition(container._viewportSize, standardOffset, resultSize);
                sprite.Scale = sizeSetup.Scale;
                yield return (targetPosition, sprite);
            }
        }
    }

    private static Vector2 CalculateRandomPosition(Vector2 viewportSize, Vector2 defaultOffset, Vector2 rightBottomOffset)
    {
        return defaultOffset + Vec2.Random() * (viewportSize - defaultOffset * 2f - rightBottomOffset);
    }
}
