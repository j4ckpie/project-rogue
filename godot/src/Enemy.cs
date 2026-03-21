using Godot;
using System;

public partial class Enemy : Character
{
	[ExportGroup("Base Variables")]
	[Export]
	private float _knockbackDeceleration = 600.0f;

	private Vector2 _knockbackVelocity = Vector2.Zero;
	
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
    	MoveAndSlide();
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

    protected override void Death()
    {
        QueueFree();
    }

    protected override void UpdateSpriteDirection()
	{
		if(GlobalPosition.X - Player.currentPlayerPositionRef.X > 0)
		{
			_playerSprite.FlipH = true;
		}
		else if(GlobalPosition.X - Player.currentPlayerPositionRef.X < 0)
		{
			_playerSprite.FlipH = false;
		}
	}
	
	public override void ApplyKnockback(float amount, Vector2 direction)
	{
		_knockbackVelocity = direction * amount;
	}
}
