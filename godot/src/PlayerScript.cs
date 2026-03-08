using Godot;
using System;

[GlobalClass]
public partial class PlayerScript : CharacterBody2D
{
	public enum MovementMode { IDLE, WALK, SPRINT, ATTACK, HEAVY_ATTACK, SHOOT }

	[ExportGroup("Nodes")]
	[Export]
	private Sprite2D _playerSprite;
	[Export]
	private AnimationPlayer _animationPlayer;
	[Export]
	private Control _crosshairSprite;

	[ExportGroup("Settings")]
	[Export]
	private float _movementSpeed = 75.0f;
	[Export]
	private float _sprintSpeedMultiplier = 1.75f;
	[Export]
	private float _sneakSpeedMultiplier = 0.5f;
	[Export]
	private float _acceleration = 12.0f;
	[Export]
	private float _friction = 7.0f;

	private float _health = 100.0f;
	private Vector2 _direction = Vector2.Zero;
	private Vector2 _desiredVelocity = Vector2.Zero;
	private bool _isSprinting = false;
	private bool _isSneaking = false;
	private MovementMode _movementMode = MovementMode.IDLE;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Hidden;
	}

    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed("attack")) Attack();
		if(@event.IsActionPressed("heavy_attack")) HeavyAttack();
		if(@event.IsActionPressed("shoot")) Shoot();
    }


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_crosshairSprite.GlobalPosition = GetGlobalMousePosition();
	}

    public override void _PhysicsProcess(double delta)
    {
		_direction.X = Input.GetActionStrength("right") - Input.GetActionStrength("left");
		_direction.Y = Input.GetActionStrength("down") - Input.GetActionStrength("up");
		_isSprinting = Input.IsActionPressed("sprint");
		_isSneaking = Input.IsActionPressed("sneak") && !_isSprinting;

		float currentSpeedMultiplier = 1.0f;
		if(_isSprinting) currentSpeedMultiplier = _sprintSpeedMultiplier;
		else if(_isSneaking) currentSpeedMultiplier = _sneakSpeedMultiplier;

		if (_movementMode == MovementMode.IDLE || _movementMode == MovementMode.WALK)
    	{
        	_movementMode = _direction.Length() > 0 ? MovementMode.WALK : MovementMode.IDLE;
    	}

		_desiredVelocity = _direction.Normalized() * _movementSpeed * currentSpeedMultiplier;

		if(_direction.Length() > 0)
		{
			Velocity = Velocity.Lerp(_desiredVelocity, _acceleration * (float)delta);
		}
		else
		{
			Velocity = Velocity.Lerp(_desiredVelocity, _friction * (float)delta);
		}


        MoveAndSlide();
		UpdateMovementAnimation();
    }

	public void _on_animation_player_animation_finished(string animName)
	{
    	if(animName == "attack" || animName == "heavy_attack" || animName == "shoot")
    	{
        	_movementMode = MovementMode.IDLE;
    	}
	}

	private void UpdateMovementAnimation()
	{
		UpdateSpriteDirection();
		if(_movementMode == MovementMode.IDLE || _movementMode == MovementMode.WALK)
		{
			if(_desiredVelocity.Length() ==  0)
			{
				_animationPlayer.Play("idle");
			}
			else if(_desiredVelocity.Length() > 0)
			{
				_animationPlayer.Play("walk");
				if(_isSprinting && !_isSneaking) _animationPlayer.SpeedScale = 1.075f;
				else if(!_isSprinting && _isSneaking) _animationPlayer.SpeedScale = 0.5f;
				else _animationPlayer.SpeedScale = 1.0f;
			}
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

	private void Attack()
	{
		_movementMode = MovementMode.ATTACK;
		_animationPlayer.Play("attack");
	}

	private void HeavyAttack()
	{
		_movementMode = MovementMode.HEAVY_ATTACK;
		_animationPlayer.Play("heavy_attack");
	}

	private void Shoot()
	{
		_movementMode = MovementMode.SHOOT;
		_animationPlayer.Play("shoot");
	}

}
