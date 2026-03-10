using Godot;
using System;

[GlobalClass]
public abstract partial class Character : CharacterBody2D
{
	public enum MovementMode { IDLE, WALK, SPRINT, ATTACK, HEAVY_ATTACK, SHOOT }

	[ExportGroup("Nodes")]
	[Export]
	protected Sprite2D _playerSprite;
	[Export]
	protected AnimationPlayer _animationPlayer;

	[ExportGroup("Base Variables")]
	[Export]
	protected float _health = 100.0f;
	[Export]
	protected float _movementSpeed = 75.0f;
	[Export]
	protected float _currentSpeedMultiplier = 1.0f;
	[Export]
	protected float _sprintSpeedMultiplier = 1.75f;
	[Export]
	protected float _sneakSpeedMultiplier = 0.5f;
	[Export]
	protected float _acceleration = 12.0f;
	[Export]
	protected float _friction = 7.0f;
	[Export]
	protected string _animIdleName = "idle";
	[Export]
	protected string _animWalkName = "walk";
	[Export]
	protected string _animAttackName = "attack";
	[Export]
	protected string _animHeavyAttackName = "heavy_attack";
	[Export]
	protected string _animShootName = "shoot";

	protected Vector2 _direction = Vector2.Zero;
	protected Vector2 _desiredVelocity = Vector2.Zero;
	protected bool _isSprinting = false;
	protected bool _isSneaking = false;
	protected MovementMode _movementMode = MovementMode.IDLE;

	public override void _Ready()
	{
		
	}

	public override void _Process(double delta)
	{

	}

    public override void _PhysicsProcess(double delta)
    {
		if(_isSprinting && _movementMode != MovementMode.SHOOT && _movementMode != MovementMode.HEAVY_ATTACK) _currentSpeedMultiplier = _sprintSpeedMultiplier;
		else if(_isSneaking && _movementMode != MovementMode.SHOOT && _movementMode != MovementMode.HEAVY_ATTACK) _currentSpeedMultiplier = _sneakSpeedMultiplier;

		if (_movementMode == MovementMode.IDLE || _movementMode == MovementMode.WALK)
    	{
        	_movementMode = _direction.Length() > 0 ? MovementMode.WALK : MovementMode.IDLE;
			if(!_isSprinting && !_isSneaking) _currentSpeedMultiplier = 1.0f;
    	}

		_desiredVelocity = _direction.Normalized() * _movementSpeed * _currentSpeedMultiplier;

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
    	if(animName == _animAttackName)
    	{
        	_movementMode = MovementMode.IDLE;
    	}
		else if(animName == _animHeavyAttackName)
		{
			_movementMode = MovementMode.IDLE;
			_currentSpeedMultiplier = 1.0f;
		}
		else if(animName == _animShootName)
		{
			SpawnProjectile();
			_movementMode = MovementMode.IDLE;
			_currentSpeedMultiplier = 1.0f;
		}
	}

	private void UpdateMovementAnimation()
	{
		UpdateSpriteDirection();
		if(_movementMode == MovementMode.IDLE || _movementMode == MovementMode.WALK)
		{
			if(_desiredVelocity.Length() ==  0)
			{
				_animationPlayer.Play(_animIdleName);
			}
			else if(_desiredVelocity.Length() > 0)
			{
				_animationPlayer.Play(_animWalkName);
				if(_isSprinting && !_isSneaking) _animationPlayer.SpeedScale = 1.075f;
				else if(!_isSprinting && _isSneaking) _animationPlayer.SpeedScale = 0.5f;
				else _animationPlayer.SpeedScale = 1.0f;
			}
		}
	}

	protected abstract void UpdateSpriteDirection();

	protected virtual bool Attack()
	{
		if(_movementMode == MovementMode.ATTACK) return false;
		_movementMode = MovementMode.ATTACK;
		_animationPlayer.Play(_animAttackName);
		return true;
	}

	protected virtual bool HeavyAttack()
	{
		if(_movementMode == MovementMode.HEAVY_ATTACK) return false;
		_movementMode = MovementMode.HEAVY_ATTACK;
		_animationPlayer.Play(_animHeavyAttackName);
		_currentSpeedMultiplier = 0.1f;
		return true;
	}

	protected abstract void SpawnProjectile();

	protected virtual bool Shoot()
	{
		if(_movementMode == MovementMode.SHOOT) return false;
		_movementMode = MovementMode.SHOOT;
		_animationPlayer.Play(_animShootName);
		_currentSpeedMultiplier = 0.25f;
		return true;
	}

}
