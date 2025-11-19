using Godot;
using System;

public static class Vec2
{
    private static readonly Random s_random = System.Random.Shared;

    public static Vector2 Random(float from, float to) => new(
        from + (s_random.NextSingle() * (to - from)),
        from + (s_random.NextSingle() * (to - from)));
    public static Vector2 Random() => new(s_random.NextSingle(), s_random.NextSingle());
}

public static class Property
{
    public static string Position => Node2D.PropertyName.Position;
    public static string Scale => Node2D.PropertyName.Scale;
    public static string ZIndex => CanvasItem.PropertyName.ZIndex;
}