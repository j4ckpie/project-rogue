using System;
using Godot;

public partial class Camera : Camera2D
{
    [Export]
    protected Node2D _target;
    [Export]
    protected float _followSpeed = 3.0f;
    [Export]
    protected float _mouseInfluence = 0.1f; 
    [Export]
    protected float _maxMouseOffset = 75.0f;

    protected float _shakeIntensity = 0.0f;
    protected RandomNumberGenerator _rand;

    public override void _Ready()
    {
        _rand = new RandomNumberGenerator();
    }

    public override void _Process(double delta)
    {
        if(_target != null)
        {
            Vector2 playerPos = _target.GlobalPosition;
            Vector2 mousePos = GetGlobalMousePosition();
            Vector2 mouseOffset = (mousePos - playerPos) * _mouseInfluence;
            mouseOffset = mouseOffset.LimitLength(_maxMouseOffset);
            Vector2 targetPosition = playerPos + mouseOffset;
            GlobalPosition = GlobalPosition.Lerp(targetPosition, _followSpeed * (float)delta);
        }

        if(_shakeIntensity > 0)
        {
            _shakeIntensity = Mathf.Lerp(_shakeIntensity, 0, (float)delta * 5.0f);
            Offset = new Vector2(_rand.RandfRange(-_shakeIntensity, _shakeIntensity), _rand.RandfRange(-_shakeIntensity, _shakeIntensity));
        }
    }

    public void StartCameraShake(float intensity)
    {
        _shakeIntensity = intensity;
    }
}