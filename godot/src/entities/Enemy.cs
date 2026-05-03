using Godot;
using System;

public partial class Enemy : Character
{
	[ExportGroup("AI Settings")]
	[Export]
	public float ChaseRange { get; protected set; } = 250.0f;
	[Export]
	public float AttackRange { get; protected set; } = 45.0f;
	[Export]
	public float AttackCooldown { get; protected set; } = 2.0f;

	[ExportGroup("Nodes")]
	[Export]
	public Timer SlownessTimer { get; protected set; }
	[Export]
	public Label DisplayedLvl { get; protected set; }
	[Export]
	public PackedScene DamagePopUp { get; protected set; }

	[ExportGroup("Base Variables")]
	[Export]
	public float KnockbackDeceleration { get; protected set; } = 600.0f;

	protected Vector2 _knockbackVelocity = Vector2.Zero;
	protected float _currentCooldown = 0.0f;

    public override void _Ready()
    {
		// i have no clue why this exact export isn't working
		if(DamagePopUp == null)
    	{
        	DamagePopUp = GD.Load<PackedScene>("res://scenes/ui/DamagePopUp.tscn");
    	}
        base._Ready();
    }

	public override void _PhysicsProcess(double delta)
	{
		if(!IsDead)
		{
			if(_knockbackVelocity != Vector2.Zero)
    		{
        		Velocity = _knockbackVelocity;
        		_knockbackVelocity = _knockbackVelocity.MoveToward(Vector2.Zero, KnockbackDeceleration * (float)delta);
    		}
    		else
    		{
				Velocity = Vector2.Zero;
        		if (_currentState is IdleState || _currentState is MoveState)
        		{
            		ThinkAndAct(delta);
        		}
    		}
    		base._PhysicsProcess(delta);
		}
	}

    public override void _on_animation_player_animation_finished(string animName)
	{
		base._on_animation_player_animation_finished(animName);
		if(animName == AnimDeathName)
		{
			Tween deleteTween = CreateTween();
			deleteTween.TweenInterval(2.0f);
			deleteTween.TweenCallback(Callable.From(QueueFree));
		}
	}

	public void _on_light_attack_area_body_entered(Node2D body)
    {
        if(body is IDamageable damageable) CheckUpdateXp(damageable.TakeDamage(BaseDamage));
    }

    public void _on_heavy_attack_area_body_entered(Node2D body)
    {
        if(body is IDamageable damageable) CheckUpdateXp(damageable.TakeDamage(BaseHeavyDamage));
    }

	public void _on_leveled_up(int amount)
	{
		DisplayedLvl.Text = $"{amount.ToString()} lvl";
	}

	public override void ApplyKnockback(float amount, Vector2 direction)
	{
		_knockbackVelocity = direction * amount;
	}

    public override void ApplySlowness(float amount)
    {
        _statusSpeedMultiplier = amount;
		IsStunned = true;
		_currentCooldown = AttackCooldown;
		SlownessTimer.Start();
    }

	public void _on_slowness_timer_timeout()
	{
		_statusSpeedMultiplier = _baseStatusSpeedMultiplier;
		IsStunned = false;
	}

	public override void AfterAttack()
	{
	}

	public override void AfterHeavyAttack()
	{
	}

	public override void AfterShoot()
	{
	}

    protected override void SpawnDamagePopUp()
    {
        PopUp popup = DamagePopUp.Instantiate<PopUp>();
    
        GetTree().CurrentScene.AddChild(popup);
	
        popup.GlobalPosition = GlobalPosition + new Vector2((float)GD.RandRange(-30, 15), (float)GD.RandRange(-30, 0));
        popup.Start(LastDamageTaken, "-", "HP");
    }

	protected override void CheckUpdateXp(float amount)
    {
        Xp += amount;
        if(Xp >= 25.0f)
        {
            // todo: upgrades etc
			Health *= 1.25f;
			BaseDamage *= 1.25f;
			BaseHeavyDamage *= 1.25f;
			MovementSpeed *= 1.025f;
            float xpDiff = Xp - 100.0f; // todo change 100.0f to xp stages
            Xp = xpDiff;
            Lvl++;
            EmitSignal(SignalName.LeveledUp, Lvl);
        }
    }

	// protected override void Death()
	// {
	// 	base.Death();
	// }

    protected override void UpdateSpriteDirection()
	{
		if(GlobalPosition.X - Player.CurrentPlayerPosition.X > 0)
		{
			TargetSprite.FlipH = true;
			AttackAreas.Scale = new Vector2(-1, 1);
			DustParticles.Direction = new Vector2(1, 0);
		}
		else if(GlobalPosition.X - Player.CurrentPlayerPosition.X < 0)
		{
			TargetSprite.FlipH = false;
			AttackAreas.Scale = new Vector2(1, 1);
			DustParticles.Direction = new Vector2(-1, 0);
		}
	}

	protected void ThinkAndAct(double delta)
    {
        if(_currentCooldown > 0) _currentCooldown -= (float)delta;
		float distanceToPlayer = (Player.CurrentPlayerPosition - GlobalPosition).Length();
		if(distanceToPlayer <= AttackRange)
		{
			//_direction = Vector2.Zero;
			Func<bool> attackMethod;
			if(GD.RandRange(0, 1) < 0.5) attackMethod = HeavyAttack;
			else attackMethod = Attack;
			if(_currentCooldown <= 0 && !IsStunned)
			{
				if(attackMethod())
				{
					_currentCooldown = AttackCooldown;
				}
			}
		}
		else if(distanceToPlayer <= ChaseRange)
		{
			Direction = (Player.CurrentPlayerPosition - GlobalPosition).Normalized();
		}
		else
		{
			Direction = Vector2.Zero;
		}
    }
}
