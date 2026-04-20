using Godot;
using System;
using System.Runtime.InteropServices;

[GlobalClass]
public abstract partial class Character : CharacterBody2D, IDamageable
{
	public enum MovementMode { IDLE, WALK, SPRINT, ATTACK, HEAVY_ATTACK, SHOOT, TAKE_DAMAGE, DEATH}	

	[ExportGroup("Nodes")]
	[Export]
	public Sprite2D TargetSprite { get; private set; }
	[Export]
	public AnimationPlayer AnimPlayer { get; private set; }
	[Export]
    public Node2D AttackAreas { get; private set; }

	[ExportGroup("Base Variables")]
	[Export]
	public float Health { get; protected set; } = 100.0f;
	[Export]
    public float BaseDamage { get; protected set; } = 25.0f;
    [Export]
    public float BaseHeavyDamage { get; protected set; } = 40.0f;
	[Export]
	public float BaseXpAmountDropped { get; protected set; } = 25.0f;
	[Export]
	public float MovementSpeed { get; protected set; } = 75.0f;
	[Export]
	public string AnimIdleName { get; private set; } = "idle";
	[Export]
	public string AnimWalkName { get; private set; } = "walk";
	[Export]
	public string AnimAttackName { get; private set; } = "attack";
	[Export]
	public string AnimHeavyAttackName { get; private set; } = "heavy_attack";
	[Export]
	public string AnimShootName { get; private set; } = "shoot";
	[Export]
	public string AnimDeathName { get; private set; } = "death";
	[Export]
	public string AnimTakeDamageName { get; private set; } = "take_damage";

	public int Lvl { get; protected set; } = 1;
	public float Xp { get; protected set; } = 0.0f;
	public bool IsSprinting { get; protected set; } = false;
	public bool IsSneaking { get; protected set; } = false;
	public bool IsStunned { get; protected set; } = false;
	public float FinalSpeed => MovementSpeed * CurrentSpeedMultiplier * _statusSpeedMultiplier;
	protected State _currentState;
	public Vector2 Direction { get; protected set; } = Vector2.Zero;
	protected Vector2 _desiredVelocity = Vector2.Zero;
	public float CurrentSpeedMultiplier { get; set; } = 1.0f;
	protected float _sprintSpeedMultiplier = 1.75f;
	protected float _sneakSpeedMultiplier = 0.5f;
	protected float _acceleration = 12.0f;
	protected float _friction = 7.0f;
	protected float _maxHealth = 100.0f;
	protected float _statusSpeedMultiplier = 1.0f;
	protected float _baseStatusSpeedMultiplier = 1.0f;
	protected MovementMode _movementMode = MovementMode.IDLE;
	protected Tween _fadeTween;

	[Signal]
    public delegate void XpChangedEventHandler(float amount);
	[Signal]
    public delegate void LeveledUpEventHandler(int amount);

	public override void _Ready()
	{
		ChangeState(new IdleState(this));
	}

	public override void _Process(double delta)
	{

	}

    public override void _PhysicsProcess(double delta)
    {
		if (_movementMode == MovementMode.DEATH) return;
		if(IsSprinting && _movementMode != MovementMode.SHOOT && _movementMode != MovementMode.HEAVY_ATTACK) CurrentSpeedMultiplier = _sprintSpeedMultiplier;
		else if(IsSneaking && _movementMode != MovementMode.SHOOT && _movementMode != MovementMode.HEAVY_ATTACK) CurrentSpeedMultiplier = _sneakSpeedMultiplier;

		if (_movementMode == MovementMode.IDLE || _movementMode == MovementMode.WALK)
    	{
        	_movementMode = Direction.Length() > 0 ? MovementMode.WALK : MovementMode.IDLE;
			if(!IsSprinting && !IsSneaking) CurrentSpeedMultiplier = 1.0f;
    	}

		_desiredVelocity = Direction.Normalized() * FinalSpeed;

		if(Direction.Length() > 0)
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
    	if(animName == AnimAttackName)
    	{
        	_movementMode = MovementMode.IDLE;
    	}
		else if(animName == AnimHeavyAttackName)
		{
			AfterHeavyAttack();
			_movementMode = MovementMode.IDLE;
			CurrentSpeedMultiplier = 1.0f;
		}
		else if(animName == AnimShootName)
		{
			AfterShoot();
			_movementMode = MovementMode.IDLE;
			CurrentSpeedMultiplier = 1.0f;
		}
		else if(animName == AnimTakeDamageName)
		{
			_movementMode = MovementMode.IDLE;
		}
		else if(animName == AnimDeathName)
		{
			PlayFadeAnimation(TargetSprite, 0.0f, 2.0f);
		}
	}

	public virtual float TakeDamage(float amount)
    {
		if(_movementMode == MovementMode.DEATH) return 0;
        Health -= amount;
		if(Health <= 0)
		{
			Death();
			return CalculateDroppedXp();
		}
		else
		{
			_movementMode = MovementMode.TAKE_DAMAGE;
			AnimPlayer.Play(AnimTakeDamageName);
			return 0;
		}
    }

	public void ChangeState(State state)
	{
		_currentState.Exit();
		_currentState = state;
		_currentState.Enter();
	}

	public virtual void ApplyKnockback(float amount, Vector2 direction)
	{
		
	}

	public virtual void ApplySlowness(float amount)
	{
		
	}

	public abstract void AfterAttack();
	public abstract void AfterHeavyAttack();
	public abstract void AfterShoot();

	protected abstract void CheckUpdateXp(float amount);

	private void UpdateMovementAnimation()
	{
		UpdateSpriteDirection();
		if(_movementMode == MovementMode.IDLE || _movementMode == MovementMode.WALK)
		{
			if(_desiredVelocity.Length() ==  0)
			{
				AnimPlayer.Play(AnimIdleName);
			}
			else if(_desiredVelocity.Length() > 0)
			{
				AnimPlayer.Play(AnimWalkName);
				if(IsSprinting && !IsSneaking) AnimPlayer.SpeedScale = 1.075f;
				else if(!IsSprinting && IsSneaking) AnimPlayer.SpeedScale = 0.5f;
				else AnimPlayer.SpeedScale = 1.0f;
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
		AnimPlayer.Play(AnimAttackName);
		return true;
	}

	protected virtual bool HeavyAttack()
	{
		if(_movementMode == MovementMode.HEAVY_ATTACK || _movementMode == MovementMode.HEAVY_ATTACK || _movementMode == MovementMode.SHOOT) return false;
		_movementMode = MovementMode.HEAVY_ATTACK;
		AnimPlayer.Play(AnimHeavyAttackName);
		CurrentSpeedMultiplier = 0.1f;
		return true;
	}

	protected virtual bool Shoot()
	{
		if(_movementMode == MovementMode.SHOOT || _movementMode == MovementMode.HEAVY_ATTACK || _movementMode == MovementMode.SHOOT) return false;
		_movementMode = MovementMode.SHOOT;
		AnimPlayer.Play(AnimShootName);
		CurrentSpeedMultiplier = 0.25f;	
		return true;
	}

	protected virtual void Death()
	{
		_movementMode = MovementMode.DEATH;
		AnimPlayer.Play(AnimDeathName);
		Velocity = Vector2.Zero;
		SetDeferred(CollisionObject2D.PropertyName.CollisionLayer, 0u);
		SetDeferred(CollisionObject2D.PropertyName.CollisionMask, 0u);
	}

	protected float CalculateDroppedXp()
	{
		RandomNumberGenerator rand = new RandomNumberGenerator();
		float low = BaseXpAmountDropped - 5.0f;
		float high = BaseXpAmountDropped + 5.0f;
		float mulitplier = 1 + (1 - 1 / (float)Lvl);
		return rand.RandfRange(low, high) * mulitplier;
	}
}
