using Godot;
using System;

[GlobalClass]
public partial class PlayerScript : CharacterBody2D
{
	public enum MovementMode { IDLE, WALK, SPRINT, ATTACK, SHOOT }

	[ExportGroup("Nodes")]
	[Export]
	private Sprite2D _playerSprite;
	[Export]
	private AnimationPlayer _animationPlayer;

	[ExportGroup("Settings")]
	[Export]
	private float _movementSpeed = 100.0f;
	[Export]
	private float _acceleration = 12.0f;
	[Export]
	private float _friction = 7.0f;

	private float _health = 100.0f;
	private Vector2 _direction = Vector2.Zero;
	private Vector2 _desiredVelocity = Vector2.Zero;
	private bool _isSprinting = false;
	private MovementMode _movementMode = MovementMode.IDLE;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
		_direction.X = Input.GetActionStrength("right") - Input.GetActionStrength("left");
		_direction.Y = Input.GetActionStrength("down") - Input.GetActionStrength("up");
		_isSprinting = Input.GetActionStrength("sprint") > 0 ? true : false;

		if(_isSprinting) _desiredVelocity = _direction.Normalized() * _movementSpeed * 1.75f;
		else _desiredVelocity = _direction.Normalized() * _movementSpeed;

		if(_direction.Length() > 0)
		{
			Velocity = Velocity.Lerp(_desiredVelocity, _acceleration * (float)delta);
		}
		else
		{
			Velocity = Velocity.Lerp(_desiredVelocity, _friction * (float)delta);
		}


        MoveAndSlide();
		UpdatePlayerAnimation();
    }

	private void UpdatePlayerAnimation()
	{
		UpdateSpriteDirection();
		if(_desiredVelocity.Length() ==  0)
		{
			_movementMode = MovementMode.IDLE;
			_animationPlayer.Play("idle");
		}
		else if(_desiredVelocity.Length() > 0)
		{
			_movementMode = MovementMode.WALK;
			_animationPlayer.Play("walk");
			if(_isSprinting) _animationPlayer.SpeedScale = 1.075f;
			else _animationPlayer.SpeedScale = 1.0f;
		}
	}

	private void UpdateSpriteDirection()
	{
		if(GlobalPosition.X - GetGlobalMousePosition().X > 0)
		{
			_playerSprite.FlipH = true;
		}
		else if(GlobalPosition.X - GetGlobalMousePosition().X < 0)
		{
			_playerSprite.FlipH = false;
		}
	}

}
