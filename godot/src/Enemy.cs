using Godot;
using System;

public partial class Enemy : Character
{
	[ExportGroup("Base Variables")]
	[Export]
	protected float _knockbackDeceleration = 600.0f;

	protected Vector2 _knockbackVelocity = Vector2.Zero;
	
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
        	// TODO: AI MOVE
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

	protected override void Death()
	{
		base.Death();

	}

	protected override void AfterAttack()
	{
		throw new NotImplementedException();
	}

	protected override void AfterHeavyAttack()
	{
		throw new NotImplementedException();
	}

	protected override void AfterShoot()
	{
		throw new NotImplementedException();
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
	
	public override void ApplyKnockback(float amount, Vector2 direction)
	{
		_knockbackVelocity = direction * amount;
	}
}
