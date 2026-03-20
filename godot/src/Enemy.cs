using Godot;
using System;

public partial class Enemy : Character
{
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
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
		UpdateZIndex();
	}

	private void UpdateZIndex()
	{
		
	}

}
