using Godot;
using System;
using System.Linq;

public partial class SpriteContainer : Node2D
{
    [Export]
    public PackedScene TrashImageScene { get; set; } = default!;

    public override void _Ready()
    {
        if (TrashImageScene is null) throw new InvalidOperationException($"Trash image scene is not set.");
        var textures = Enumerable.Range(1, 14).Select(i => (i, GD.Load<Texture2D>($"res://images/trash_{i}.png")));
        foreach (var (index, texture) in textures)
        {
            var node = TrashImageScene.Instantiate<TrashImage>();
            node.Name = $"trash_{index}";
            node.Texture = texture;
            AddChild(node);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
