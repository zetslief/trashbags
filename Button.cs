using Godot;
using System;

public partial class Button : Godot.Button
{
    public override void _Pressed()
    {
        GD.Print("Hello, Godot!");
    }
}
