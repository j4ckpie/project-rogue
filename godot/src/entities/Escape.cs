using Godot;
using System;

public partial class Escape : Node2D
{
    public static Vector2 CurrentEscapePosition { get; private set; }

    public override void _Ready()
    {
        CurrentEscapePosition = GlobalPosition;
    }
}
