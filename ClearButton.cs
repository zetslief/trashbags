using Godot;
using System;

public partial class ClearButton : Button
{
    [Export]
    public SpriteContainer SpriteContainer { get; set; } = default!;

    public override void _Ready()
    {
        if (SpriteContainer is null) throw new InvalidOperationException($"SpriteContainer is not set.");
    }

    public override void _Pressed() => GD.Print("Clear");
}
