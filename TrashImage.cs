using Godot;
using System;

public partial class TrashImage : StaticBody2D
{
    const float MouseInteractionDuration = 0.15f;
    private Vector2 _initialScale;
    private bool _hovered = false;
    private Vector2 _initialPosition;
    private bool _selected = false;
    private Sprite2D _sprite = default!;

    [Export]
    private Texture2D? Texture { get; set; }

    public override void _Ready()
    {
        if (Texture is null) throw new InvalidOperationException();
        _sprite = GetNode<Sprite2D>("Sprite");
        _sprite.Texture = Texture;
        var collider = GetNode<CollisionShape2D>("Collider");
        collider.Shape = new RectangleShape2D()
        {
            Size = _sprite.GetRect().Size
        };
    }

    public override void _MouseEnter()
    {
        _hovered = true;
        if (_selected) return;
        _initialScale = Scale;
    }

    public override void _MouseExit()
    {
        _hovered = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (!_hovered) return;
        if (@event is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left && mb.IsPressed())
        {
            _selected = !_selected;
            var sprite = GetNode<Sprite2D>("Sprite");
            var viewportSize = GetViewportRect().Size;
            var tween = CreateTween().SetParallel(true);
            if (_selected)
            {
                _initialPosition = Position;
                var currentScale = Scale;
                var currentSpriteSize = sprite.GetRect().Size * currentScale;
                var targetSpriteWidth = viewportSize.X * 0.4f;
                var multiplier = targetSpriteWidth / currentSpriteSize.X;
                var targetPosition = new Vector2((viewportSize.X - targetSpriteWidth) / 2, viewportSize.Y * 0.009f);
                tween.TweenProperty(this, (string)PropertyName.Scale, Scale * multiplier, 1);
                tween.TweenProperty(this, (string)PropertyName.Position, targetPosition, 1);
                tween.TweenProperty(this, (string)PropertyName.ZIndex, 1000, 1);
            }
            else
            {
                tween.TweenProperty(this, (string)PropertyName.Scale, _initialScale, 1);
                tween.TweenProperty(this, (string)PropertyName.Position, _initialPosition, 1);
                tween.TweenProperty(this, (string)PropertyName.ZIndex, 0, 1);
            }
        }
    }

    public static void SetSize(TrashImage trashImage, float height)
    {
        var sprite = trashImage._sprite;
        var currentScale = trashImage.Scale;
        var currentSpriteSize = sprite.GetRect().Size * currentScale;
        var resultScale = CalculateScale(currentScale, currentSpriteSize, height);
        trashImage.Scale = resultScale;
    }

    private static Vector2 CalculateScale(Vector2 currentScale, Vector2 currentSize, float requiredHeight)
    {
        var multiplier = requiredHeight / currentSize.Y;
        return currentScale * multiplier;
    }
}