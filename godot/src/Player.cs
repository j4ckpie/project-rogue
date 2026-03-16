using Godot;
using Godot.NativeInterop;
using System;
using System.ComponentModel;

public partial class Player : Character
{
    [ExportGroup("Nodes")]
    [Export]
	private PackedScene _arrowSprite;
	[Export]
	private Control _crosshairSprite;
    [Export]
    private Camera _camera;
    [Export]
    private TextureProgressBar _staminaBar;
    [Export]
    private Timer _staminaRegenTimer;
    [Export]
    private Timer _heavyAttackDelayTimer;

    [ExportGroup("Base Variables")]
    [Export]
    private float _staminaMax = 100.0f;
    [Export]
    private float _staminaRate = 25.0f;
    [Export]
    private float _staminaDelay = 1.0f;
    [Export]
    private float _attackShakeIntensity = 1.25f;
    [Export]
    private float _heavyAttackShakeIntensity = 5.0f;
    [Export]
    private float _shootShakeIntensity = 3.0f;
	[Export]
    private string _actionUpName = "up";
    [Export]
    private string _actionDownName = "down";
    [Export]
    private string _actionLeftName = "left";
    [Export]
    private string _actionRightName = "right";
    [Export]
    private string _actionSprintName = "sprint";
    [Export]
    private string _actionSneakName = "sneak";
    [Export]
    private string _actionAttackName = "attack";
    [Export]
    private string _actionHeavyAttackName = "heavy_attack";
    [Export]
    private string _actionShootName = "shoot";

    private float _staminaCurrent = 100.0f;
    private bool _canRegenStamina = true;
    private bool _canHeavyAttack = true;
    private Tween _fadeTween;
    
    public override void _Ready()
	{
        base._Ready();

		Engine.MaxFps = 0;	// TODO: PLACEHOLDER
		Input.MouseMode = Input.MouseModeEnum.Hidden;
        _staminaBar.Value = _staminaCurrent;
	}

	public override void _Process(double delta)
	{
        base._Process(delta);

		DisplayServer.WindowSetTitle("Rogue | " + Engine.GetFramesPerSecond() + " fps");	// TODO: PLACEHOLDER

        if(_canRegenStamina && _staminaCurrent < _staminaMax)
        {
            _staminaCurrent = Mathf.MoveToward(_staminaCurrent, _staminaMax, _staminaRate * (float)delta);
            _staminaBar.Value = _staminaCurrent;
        }
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

    public void _on_stamina_timer_timeout()
    {
        _canRegenStamina = true;
        FadeStaminaBar(0.0f, 1.5f);
    }

    public void _on_heavy_attack_timer_timeout()
    {
        _canHeavyAttack = true;
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

    private bool SpendStamina(float amount)
    {
        if(_staminaCurrent - amount < 0) return false;
        _staminaCurrent = _staminaCurrent - amount;
        _staminaBar.Value = _staminaCurrent;
        _canRegenStamina = false;
        FadeStaminaBar(1.0f, 0.25f);
        _staminaRegenTimer.Stop();
        _staminaRegenTimer.Start(2.0f);
        return true;
    }

    private void FadeStaminaBar(float targetAlpha, float duration)
    {
        if(_fadeTween != null && _fadeTween.IsRunning())
        {
            _fadeTween.Kill();
        }
        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(_staminaBar, "modulate:a", targetAlpha, duration)
              .SetTrans(Tween.TransitionType.Cubic);
    }

    protected override bool Attack()
    {
        if(SpendStamina(5.0f))
        {
            _camera.StartCameraShake(_attackShakeIntensity);
            return base.Attack();
        }
        return false;
    }

    protected override void AfterAttack()
    {
    }


    protected override bool HeavyAttack()
    {
        if(_canHeavyAttack)
        {
            if(SpendStamina(30.0f))
            {
                _canHeavyAttack = false;
                _heavyAttackDelayTimer.Start();
                _camera.StartCameraShake(_attackShakeIntensity);
                return base.HeavyAttack();
            }
        }
        return false;
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
