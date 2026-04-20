using Godot;
using System;

public partial class Crosshair : Control
{
	public override void _Process(double delta)
	{
		GlobalPosition = GetGlobalMousePosition();
	}
}
