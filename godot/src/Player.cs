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
    [Export]
    protected Camera _camera;

    [ExportGroup("Base Variables")]
    [Export]
    protected float _attackShakeIntensity = 1.25f;
    [Export]
    protected float _heavyAttackShakeIntensity = 5.0f;
    [Export]
    protected float _shootShakeIntensity = 3.0f;
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
        _camera.StartCameraShake(_attackShakeIntensity);
        return base.Attack();
    }

    protected override void AfterAttack()
    {
    }


    protected override bool HeavyAttack()
    {
        _camera.StartCameraShake(_attackShakeIntensity);
        return base.HeavyAttack();
    }

    protected virtual void MidHeavyAttack()
    {
        _camera.StartCameraShake(_heavyAttackShakeIntensity);
    }

    protected override void AfterHeavyAttack()
    {
    }

    protected override bool Shoot()
    {
        return base.Shoot();
    }

    protected override void AfterShoot()
    {
        _camera.StartCameraShake(_shootShakeIntensity);
        
        Arrow arrow = _arrowSprite.Instantiate<Arrow>();

        arrow.GlobalPosition = GlobalPosition;

        Vector2 targetPos = GetGlobalMousePosition();
        Vector2 direction = (targetPos - GlobalPosition).Normalized();

        arrow.Rotation = direction.Angle();

        GetTree().CurrentScene.AddChild(arrow);
    }
}
