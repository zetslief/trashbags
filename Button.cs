using Godot;
using System;

public partial class Button : Godot.Button
{
    [Export]
    public SpriteContainer SpriteContainer { get; set; } = default!;

    public override void _Ready()
    {
        if (SpriteContainer is null) throw new InvalidOperationException($"SpriteContainer is not initialized.");
    }

    public override void _Pressed() => SpriteContainer.Shuffle(SpriteContainer);
}
