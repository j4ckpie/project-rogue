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
    }
}