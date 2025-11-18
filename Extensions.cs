using Godot;
using System;

public static class Vec2
{
    private static readonly Random random = System.Random.Shared;

    public static Vector2 Random(float from, float to) => new(
        from + (random.NextSingle() * to - from),
        from + (random.NextSingle() * to - from));
}

public static class Property
{
    public static string Position => Node2D.PropertyName.Position;
}