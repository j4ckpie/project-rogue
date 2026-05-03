using Godot;
using System;

public partial class Player : Character
{
    [ExportGroup("Nodes")]
    [Export]
	public PackedScene ArrowSprite { get; private set; }
    [Export]
	public PackedScene XpPopUp { get; private set; }
	[Export]
	public Control CrosshairSprite { get; private set; }
    [Export]
    public Camera PlayerCamera { get; private set; }
    [Export]
    public TextureProgressBar StaminaBar { get; private set; }
    [Export]
    public Timer StaminaRegenTimer { get; private set; }
    [Export]
    public Timer HealthRegenTimer { get; private set; }
    [Export]
    public Timer HeavyAttackDelayTimer { get; private set; }

    [ExportGroup("Base Variables")]
	[Export]
    public string ActionUpName { get; private set; } = "up";
    [Export]
    public string ActionDownName { get; private set; } = "down";
    [Export]
    public string ActionLeftName { get; private set; } = "left";
    [Export]
    public string ActionRightName { get; private set; } = "right";
    [Export]
    public string ActionSprintName { get; private set; } = "sprint";
    [Export]
    public string ActionSneakName { get; private set; } = "sneak";
    [Export]
    public string ActionAttackName { get; private set; } = "attack";
    [Export]
    public string ActionHeavyAttackName { get; private set; } = "heavy_attack";
    [Export]
    public string ActionShootName { get; private set; } = "shoot";

    [Signal]
    public delegate void HpChangedEventHandler(float amount);

    public static Vector2 CurrentPlayerPosition { get; private set; }

    private float _staminaMax = 100.0f;
    private float _staminaRate = 25.0f;
    private float _staminaDelay = 2.0f;
    private float _healthRate = 1.0f;
    private float _healthDelay = 5.0f;
	private float _knockbackForce = 175.0f;
    private float _attackShakeIntensity = 1.25f;
    private float _heavyAttackShakeIntensity = 5.0f;
    private float _shootShakeIntensity = 3.0f;
    private float _staminaCurrent = 100.0f;
    private bool _canRegenStamina = true;
    private bool _canRegenHealth = true;
    private bool _canHeavyAttack = true;
    
    public override void _Ready()
	{
        base._Ready();

		Engine.MaxFps = 0;	// TODO: PLACEHOLDER
		Input.MouseMode = Input.MouseModeEnum.Hidden;

        GlobalPosition = Vector2.Zero;
        StaminaBar.Value = _staminaCurrent;
	}

	public override void _Process(double delta)
	{
        base._Process(delta);

		DisplayServer.WindowSetTitle("Rogue | " + Engine.GetFramesPerSecond() + " fps");	// TODO: PLACEHOLDER

        CurrentPlayerPosition = GlobalPosition;

        if(_canRegenStamina && _staminaCurrent < _staminaMax)
        {
            _staminaCurrent = Mathf.MoveToward(_staminaCurrent, _staminaMax, _staminaRate * (float)delta);
            StaminaBar.Value = _staminaCurrent;
        }

        if(_canRegenHealth && Health < _maxHealth)
        {
            Health = Mathf.MoveToward(Health, _maxHealth, _healthRate * (float)delta);
            EmitSignal(SignalName.HpChanged, Health);
        }
	}

    public override void _PhysicsProcess(double delta)
    {
        if(!IsDead)
        {
            Direction = Input.GetVector(ActionLeftName, ActionRightName, ActionUpName, ActionDownName);
		    IsSprinting = Input.IsActionPressed(ActionSprintName);
		    IsSneaking = Input.IsActionPressed(ActionSneakName) && !IsSprinting;

            base._PhysicsProcess(delta);
        }
    }
    
    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed(ActionAttackName)) Attack();
		if(@event.IsActionPressed(ActionHeavyAttackName)) HeavyAttack();
		if(@event.IsActionPressed(ActionShootName)) Shoot();
    }

    public void _on_stamina_timer_timeout()
    {
        _canRegenStamina = true;
        FadeStaminaBar(0.0f, 1.5f);
    }

    public void _on_health_timer_timeout()
    {
        _canRegenHealth = true;
    }

    public void _on_heavy_attack_timer_timeout()
    {
        _canHeavyAttack = true;
    }

    public void _on_light_attack_area_body_entered(Node2D body)
    {
        if(body is IDamageable damageable) CheckUpdateXp(damageable.TakeDamage(BaseDamage));
    }

    public void _on_heavy_attack_area_body_entered(Node2D body)
    {
        if(body is IDamageable damageable) CheckUpdateXp(damageable.TakeDamage(BaseHeavyDamage));
    }

    public void _on_knockback_area_body_entered(Node2D body)
    {
        if(body is IDamageable damageable)
        {
            Vector2 knockbackDirection = (body.GlobalPosition - GlobalPosition).Normalized();
            damageable.ApplyKnockback(_knockbackForce, knockbackDirection);
        }
    }

    public override float TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        EmitSignal(SignalName.HpChanged, Health);
        _canRegenHealth = false;
        HealthRegenTimer.Stop();
        HealthRegenTimer.Start(_healthDelay);
        PlayerCamera.StartCameraShake(_heavyAttackShakeIntensity);
        return CalculateDroppedXp();
    }

    public override void AfterAttack()
    {
    }

    public override void AfterHeavyAttack()
    {
    }

    public override void AfterShoot()
    {
        PlayerCamera.StartCameraShake(_shootShakeIntensity);
        
        Arrow arrow = ArrowSprite.Instantiate<Arrow>();

        arrow.GlobalPosition = GlobalPosition;

        Vector2 targetPos = GetGlobalMousePosition();
        Vector2 direction = (targetPos - GlobalPosition).Normalized();

        arrow.Rotation = direction.Angle();

        GetTree().CurrentScene.AddChild(arrow);
    }

    protected override void CheckUpdateXp(float amount)
    {
        if(amount != 0)
        {
            Xp += amount;
            SpawnXpPopup(amount);
            EmitSignal(SignalName.XpChanged, Xp);
            if(Xp >= 100.0f)
            {
                // todo: upgrades etc
                float xpDiff = Xp - 100.0f; // todo change 100.0f to xp stages
                Xp = xpDiff;
                Lvl++;
                EmitSignal(SignalName.XpChanged, Xp);
                EmitSignal(SignalName.LeveledUp, Lvl);
            }
        }
    }

    protected override void UpdateSpriteDirection()
    {
        if(GlobalPosition.X - GetGlobalMousePosition().X > 0)
		{
			//TargetSprite.FlipH = true;
            TargetSprite.Scale = new Vector2(-1 ,1);
            AttackAreas.Scale = new Vector2(-1, 1);
            DustParticles.Direction = new Vector2(1, 0);
		}
		else if(GlobalPosition.X - GetGlobalMousePosition().X < 0)
		{
			//TargetSprite.FlipH = false;
            TargetSprite.Scale = new Vector2(1,1);
            AttackAreas.Scale = new Vector2(1, 1);
            DustParticles.Direction = new Vector2(-1, 0);
		}
    }

    protected override bool Attack()
    {
        if(SpendStamina(5.0f))
        {
            PlayerCamera.StartCameraShake(_attackShakeIntensity);
            return base.Attack();
        }
        return false;
    }

    protected override bool HeavyAttack()
    {
        if(_canHeavyAttack)
        {
            if(SpendStamina(30.0f))
            {
                _canHeavyAttack = false;
                HeavyAttackDelayTimer.Start();
                PlayerCamera.StartCameraShake(_attackShakeIntensity);
                return base.HeavyAttack();
            }
        }
        return false;
    }

    protected virtual void MidHeavyAttack()
    {
        PlayerCamera.StartCameraShake(_heavyAttackShakeIntensity);
    }

    protected override bool Shoot()
    {
        return base.Shoot();
    }

    // protected override void Death()
    // {
    //     // TODO: saving, ui animation etc
    //     base.Death();
    // }

    private void SpawnXpPopup(float amount)
    {
        PopUp popup = XpPopUp.Instantiate<PopUp>();
    
        GetTree().CurrentScene.AddChild(popup);
    
        popup.GlobalPosition = GlobalPosition + new Vector2((float)GD.RandRange(-30, 15), (float)GD.RandRange(-30, 0));
        popup.Start(amount, "+", "XP");
    }

    private bool SpendStamina(float amount)
    {
        if(_staminaCurrent - amount < 0) return false;
        _staminaCurrent = _staminaCurrent - amount;
        StaminaBar.Value = _staminaCurrent;
        _canRegenStamina = false;
        PlayFadeAnimation(StaminaBar, 1.0f, 0.25f);
        StaminaRegenTimer.Stop();
        StaminaRegenTimer.Start(_staminaDelay);
        return true;
    }

    private void FadeStaminaBar(float targetAlpha, float duration)
    {
        if(_fadeTween != null && _fadeTween.IsRunning())
        {
            _fadeTween.Kill();
        }
        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(StaminaBar, "modulate:a", targetAlpha, duration)
              .SetTrans(Tween.TransitionType.Cubic);
    }
}
