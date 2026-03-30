using Godot;
using System;

public partial class Enemy : Character
{
	[ExportGroup("AI Settings")]
	[Export]
	protected float _chaseRange = 200.0f;
	[Export]
	protected float _attackRange = 45.0f;
	[Export]
	protected float _attackCooldown = 2.0f;

	[ExportGroup("Base Variables")]
	[Export]
	protected float _knockbackDeceleration = 600.0f;

	protected Vector2 _knockbackVelocity = Vector2.Zero;
	protected float _currentCooldown = 0.0f;

    public override void _Ready()
    {
		_baseDamage = 10.0f;
		_baseHeavyDamage = 10.0f;
		_currentSpeedMultiplier = 1.25f;
        base._Ready();
    }

	// public override void _Input(InputEvent @event)
    // {
    //     if(@event.IsActionPressed("debug0")) Attack();
	// 	if(@event.IsActionPressed("debug1")) HeavyAttack();
    // }

	public override void _PhysicsProcess(double delta)
	{
		if(_knockbackVelocity != Vector2.Zero)
    	{
        	Velocity = _knockbackVelocity;
        	_knockbackVelocity = _knockbackVelocity.MoveToward(Vector2.Zero, _knockbackDeceleration * (float)delta);
    	}
    	else
    	{
			Velocity = Vector2.Zero;
        	if (_movementMode == MovementMode.IDLE || _movementMode == MovementMode.WALK)
        	{
            	ThinkAndAct(delta);
        	}
    	}
    	base._PhysicsProcess(delta);
	}

    public override void _on_animation_player_animation_finished(string animName)
	{
		base._on_animation_player_animation_finished(animName);
		if(animName == _animDeathName)
		{
			Tween deleteTween = CreateTween();
			deleteTween.TweenInterval(2.0f);
			deleteTween.TweenCallback(Callable.From(QueueFree));
		}
	}

	public void _on_light_attack_area_body_entered(Node2D body)
    {
        if(body is IDamageable damageable) damageable.TakeDamage(_baseDamage);
    }

    public void _on_heavy_attack_area_body_entered(Node2D body)
    {
        if(body is IDamageable damageable) damageable.TakeDamage(_baseHeavyDamage);
    }

	public override void ApplyKnockback(float amount, Vector2 direction)
	{
		_knockbackVelocity = direction * amount;
	}

	protected override void Death()
	{
		base.Death();

	}

	protected override void AfterAttack()
	{
	}

	protected override void AfterHeavyAttack()
	{
	}

	protected override void AfterShoot()
	{
	}

    protected override void UpdateSpriteDirection()
	{
		if(GlobalPosition.X - Player.currentPlayerPositionRef.X > 0)
		{
			_targetSprite.FlipH = true;
			_attackAreas.Scale = new Vector2(-1, 1);
		}
		else if(GlobalPosition.X - Player.currentPlayerPositionRef.X < 0)
		{
			_targetSprite.FlipH = false;
			_attackAreas.Scale = new Vector2(1, 1);
		}
	}

	protected void ThinkAndAct(double delta)
    {
        if(_currentCooldown > 0) _currentCooldown -= (float)delta;
		float distanceToPlayer = (Player.currentPlayerPositionRef - GlobalPosition).Length();
		if(distanceToPlayer <= _attackRange)
		{
			//_direction = Vector2.Zero;
			Func<bool> attackMethod;
			if(GD.RandRange(0, 1) < 0.5) attackMethod = HeavyAttack;
			else attackMethod = Attack;
			if(_currentCooldown <= 0)
			{
				if(attackMethod())
				{
					_currentCooldown = _attackCooldown;
				}
			}
		}
		else if(distanceToPlayer <= _chaseRange)
		{
			_direction = (Player.currentPlayerPositionRef - GlobalPosition).Normalized();
		}
		else
		{
			_direction = Vector2.Zero;
		}
    }
}
