using Godot;
using System;
using System.Runtime.InteropServices;

[GlobalClass]
public abstract partial class Character : CharacterBody2D, IDamageable
{

	[ExportGroup("Nodes")]
	[Export]
	public Sprite2D TargetSprite { get; private set; }
	[Export]
	public AnimationPlayer AnimPlayer { get; private set; }
	[Export]
    public Node2D AttackAreas { get; private set; }
	[Export]
	public CpuParticles2D DustParticles { get; protected set; }

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

	public static int Lvl { get; protected set; }
	public float Xp { get; protected set; } = 0.0f;
	public bool IsSprinting { get; protected set; } = false;
	public bool IsSneaking { get; protected set; } = false;
	public bool IsStunned { get; protected set; } = false;
	public bool IsDead { get; set; } = false;
	public float FinalSpeed => MovementSpeed * CurrentSpeedMultiplier * _statusSpeedMultiplier;
	public Vector2 Direction { get; protected set; } = Vector2.Zero;
	public float CurrentSpeedMultiplier { get; set; } = 1.0f;
	public float SprintSpeedMultiplier { get; protected set; } = 1.75f;
	public float SneakSpeedMultiplier { get; protected set; } = 0.5f;
	public float MaxHealth { get; protected set; } = 100.0f;
	public float LastDamageTaken { get; private set; }
	protected State _currentState;
	protected Vector2 _desiredVelocity = Vector2.Zero;
	protected float _acceleration = 12.0f;
	protected float _friction = 7.0f;
	protected float _statusSpeedMultiplier = 1.0f;
	protected float _baseStatusSpeedMultiplier = 1.0f;
	protected Tween _fadeTween;

	[Signal]
    public delegate void XpChangedEventHandler(float amount);
	[Signal]
    public delegate void LeveledUpEventHandler(int amount, float lvl);

	public override void _Ready()
	{
		Lvl = 1;
		ChangeState(new IdleState(this));
	}

	public override void _Process(double delta)
	{

	}

    public override void _PhysicsProcess(double delta)
    {
		_currentState?.PhysicsProcess(delta);

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
		UpdateSpriteDirection();
    }

	public virtual void _on_animation_player_animation_finished(string animName)
	{
    	_currentState?.OnAnimationFinished(animName);
	}

	public virtual float TakeDamage(float amount)
    {
		if(_currentState is DeathState) return 0;
        Health -= amount;
		LastDamageTaken = amount;
		SpawnDamagePopUp();
		if(Health <= 0)
		{
			ChangeState(new DeathState(this));
			return CalculateDroppedXp();
		}
		else
		{
			ChangeState(new TakeDamageState(this));
			return 0;
		}
    }

	public void ChangeState(State state)
	{
		_currentState?.Exit();
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

	public virtual void DeathSequence() {}

	public void PlayFadeAnimation(Node targetBody, float targetAlpha, float duration)
    {
        if(_fadeTween != null && _fadeTween.IsRunning())
        {
            _fadeTween.Kill();
        }
        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(targetBody, "modulate:a", targetAlpha, duration)
              .SetTrans(Tween.TransitionType.Cubic);
    }

	protected virtual void SpawnDamagePopUp() {}

	protected abstract void CheckUpdateXp(float amount);

	protected abstract void UpdateSpriteDirection();

	protected virtual bool Attack()
	{
		if(_currentState is AttackState || _currentState is HeavyAttackState || _currentState is ShootState || _currentState is TakeDamageState || _currentState is DeathState) return false;
		ChangeState(new AttackState(this));
		return true;
	}

	protected virtual bool HeavyAttack()
	{
		if(_currentState is AttackState || _currentState is HeavyAttackState || _currentState is ShootState || _currentState is TakeDamageState || _currentState is DeathState) return false;
		ChangeState(new HeavyAttackState(this));
		return true;
	}

	protected virtual bool Shoot()
	{
		if(_currentState is AttackState || _currentState is HeavyAttackState || _currentState is ShootState || _currentState is TakeDamageState || _currentState is DeathState) return false;
		ChangeState(new ShootState(this));
		return true;
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
