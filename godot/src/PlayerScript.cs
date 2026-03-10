using Godot;
using System;

public partial class PlayerScript : CharacterScript
{
    public override void _Ready()
	{
        base._Ready();

		Engine.MaxFps = 0;	// TODO: PLACEHOLDER
		Input.MouseMode = Input.MouseModeEnum.Hidden;
	}

	public override void _Process(double delta)
	{
        base._Process(delta);

		DisplayServer.WindowSetTitle("Rogue | " + Engine.GetFramesPerSecond() + " fps");	// TODO: PLACEHOLDER
		_crosshairSprite.GlobalPosition = GetGlobalMousePosition();
	}

    public override void _PhysicsProcess(double delta)
    {
		_direction.X = Input.GetActionStrength("right") - Input.GetActionStrength("left");
		_direction.Y = Input.GetActionStrength("down") - Input.GetActionStrength("up");
		_isSprinting = Input.IsActionPressed("sprint");
		_isSneaking = Input.IsActionPressed("sneak") && !_isSprinting;

        base._PhysicsProcess(delta);
    }
    
    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed("attack")) Attack();
		if(@event.IsActionPressed("heavy_attack")) HeavyAttack();
		if(@event.IsActionPressed("shoot")) Shoot();
    }

    protected override void UpdateSpriteDirection()
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

    protected override bool Attack()
    {
        return base.Attack();
    }

    protected override bool HeavyAttack()
    {
        return base.HeavyAttack();
    }

    protected override bool Shoot()
    {
        return base.Shoot();
    }
}
