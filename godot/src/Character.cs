using Godot;
using System;
using System.Runtime.InteropServices;

[GlobalClass]
public abstract partial class Character : CharacterBody2D, IDamageable
{
	public enum MovementMode { IDLE, WALK, SPRINT, ATTACK, HEAVY_ATTACK, SHOOT, TAKE_DAMAGE, DEATH}

	[ExportGroup("Nodes")]
	[Export]
	protected Sprite2D _targetSprite;
	[Export]
	protected AnimationPlayer _animationPlayer;
	[Export]
    protected Node2D _attackAreas;

	[ExportGroup("Base Variables")]
	[Export]
	protected float _health = 100.0f;
	[Export]
    protected float _baseDamage = 25.0f;
    [Export]
    protected float _baseHeavyDamage = 40.0f;
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
	[Export]
	protected string _animDeathName = "death";
	[Export]
	protected string _animTakeDamageName = "take_damage";

	protected Vector2 _direction = Vector2.Zero;
	protected Vector2 _desiredVelocity = Vector2.Zero;
	protected float _statusSpeedMultiplier = 1.0f;
	protected float _baseStatusSpeedMultiplier = 1.0f;
	protected bool _isSprinting = false;
	protected bool _isSneaking = false;
	protected MovementMode _movementMode = MovementMode.IDLE;
	protected Tween _fadeTween;

	public override void _Ready()
	{
		
	}

	public override void _Process(double delta)
	{

	}

    public override void _PhysicsProcess(double delta)
    {
		if (_movementMode == MovementMode.DEATH) return;
		if(_isSprinting && _movementMode != MovementMode.SHOOT && _movementMode != MovementMode.HEAVY_ATTACK) _currentSpeedMultiplier = _sprintSpeedMultiplier;
		else if(_isSneaking && _movementMode != MovementMode.SHOOT && _movementMode != MovementMode.HEAVY_ATTACK) _currentSpeedMultiplier = _sneakSpeedMultiplier;

		if (_movementMode == MovementMode.IDLE || _movementMode == MovementMode.WALK)
    	{
        	_movementMode = _direction.Length() > 0 ? MovementMode.WALK : MovementMode.IDLE;
			if(!_isSprinting && !_isSneaking) _currentSpeedMultiplier = 1.0f;
    	}

		_desiredVelocity = _direction.Normalized() * _movementSpeed * _currentSpeedMultiplier * _statusSpeedMultiplier;

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

	public virtual void _on_animation_player_animation_finished(string animName)
	{
    	if(animName == _animAttackName)
    	{
        	_movementMode = MovementMode.IDLE;
    	}
		else if(animName == _animHeavyAttackName)
		{
			AfterHeavyAttack();
			_movementMode = MovementMode.IDLE;
			_currentSpeedMultiplier = 1.0f;
		}
		else if(animName == _animShootName)
		{
			AfterShoot();
			_movementMode = MovementMode.IDLE;
			_currentSpeedMultiplier = 1.0f;
		}
		else if(animName == _animTakeDamageName)
		{
			_movementMode = MovementMode.IDLE;
		}
		else if(animName == _animDeathName)
		{
			PlayFadeAnimation(_targetSprite, 0.0f, 2.0f);
		}
	}

	public virtual void TakeDamage(float amount)
    {
		if(_movementMode == MovementMode.DEATH) return;
        _health -= amount;
		if(_health <= 0) Death();
		else
		{
			_movementMode = MovementMode.TAKE_DAMAGE;
			_animationPlayer.Play(_animTakeDamageName);
		}
    }

	public virtual void ApplyKnockback(float amount, Vector2 direction)
	{
		
	}

	public virtual void ApplySlowness(float amount)
	{
		
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

	protected void PlayFadeAnimation(Node targetBody, float targetAlpha, float duration)
    {
        if(_fadeTween != null && _fadeTween.IsRunning())
        {
            _fadeTween.Kill();
        }
        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(targetBody, "modulate:a", targetAlpha, duration)
              .SetTrans(Tween.TransitionType.Cubic);
    }

	protected abstract void UpdateSpriteDirection();

	protected virtual bool Attack()
	{
		if(_movementMode == MovementMode.ATTACK || _movementMode == MovementMode.HEAVY_ATTACK || _movementMode == MovementMode.SHOOT) return false;
		_movementMode = MovementMode.ATTACK;
		_animationPlayer.Play(_animAttackName);
		return true;
	}

	protected abstract void AfterAttack();

	protected virtual bool HeavyAttack()
	{
		if(_movementMode == MovementMode.HEAVY_ATTACK || _movementMode == MovementMode.HEAVY_ATTACK || _movementMode == MovementMode.SHOOT) return false;
		_movementMode = MovementMode.HEAVY_ATTACK;
		_animationPlayer.Play(_animHeavyAttackName);
		_currentSpeedMultiplier = 0.1f;
		return true;
	}

	protected abstract void AfterHeavyAttack();

	protected virtual bool Shoot()
	{
		if(_movementMode == MovementMode.SHOOT || _movementMode == MovementMode.HEAVY_ATTACK || _movementMode == MovementMode.SHOOT) return false;
		_movementMode = MovementMode.SHOOT;
		_animationPlayer.Play(_animShootName);
		_currentSpeedMultiplier = 0.25f;	
		return true;
	}

	protected abstract void AfterShoot();

	protected virtual void Death()
	{
		_movementMode = MovementMode.DEATH;
		_animationPlayer.Play(_animDeathName);
		Velocity = Vector2.Zero;
		SetDeferred(CollisionObject2D.PropertyName.CollisionLayer, 0u);
		SetDeferred(CollisionObject2D.PropertyName.CollisionMask, 0u);
	}
}
