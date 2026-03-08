using Godot;
using System;

[GlobalClass]
public partial class PlayerScript : CharacterBody2D
{
	float health = 100.0f;
	float movementSpeed = 100.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 direction = Vector2.Zero;
		direction.X = Input.GetActionStrength("right") - Input.GetActionStrength("left");
		direction.Y = Input.GetActionStrength("down") - Input.GetActionStrength("up");
		Velocity = direction * movementSpeed;
	}

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }

}
