using Godot;
using System;
using System.ComponentModel;

public partial class Player : Character
{
    [ExportGroup("Nodes")]
    [Export]
	protected PackedScene _arrowSprite;
	[Export]
	protected Control _crosshairSprite;

    [ExportGroup("Base Variables")]
	[Export]
    protected string _actionUpName = "up";
    [Export]
    protected string _actionDownName = "down";
    [Export]
    protected string _actionLeftName = "left";
    [Export]
    protected string _actionRightName = "right";
    [Export]
    protected string _actionSprintName = "sprint";
    [Export]
    protected string _actionSneakName = "sneak";
    [Export]
    protected string _actionAttackName = "attack";
    [Export]
    protected string _actionHeavyAttackName = "heavy_attack";
    [Export]
    protected string _actionShootName = "shoot";

    
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
	}

    public override void _PhysicsProcess(double delta)
    {
		_direction.X = Input.GetActionStrength(_actionRightName) - Input.GetActionStrength(_actionLeftName);
		_direction.Y = Input.GetActionStrength(_actionDownName) - Input.GetActionStrength(_actionUpName);
		_isSprinting = Input.IsActionPressed(_actionSprintName);
		_isSneaking = Input.IsActionPressed(_actionSneakName) && !_isSprinting;

        base._PhysicsProcess(delta);
    }
    
    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed(_actionAttackName)) Attack();
		if(@event.IsActionPressed(_actionHeavyAttackName)) HeavyAttack();
		if(@event.IsActionPressed(_actionShootName)) Shoot();
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

    protected override void SpawnProjectile()
    {
        Arrow arrow = _arrowSprite.Instantiate<Arrow>();

        arrow.GlobalPosition = GlobalPosition;

        Vector2 targetPos = GetGlobalMousePosition();
        Vector2 direction = (targetPos - GlobalPosition).Normalized();

        arrow.Rotation = direction.Angle();

        GetTree().CurrentScene.AddChild(arrow);
    }

    protected override bool Shoot()
    {
        return base.Shoot();
    }
}
