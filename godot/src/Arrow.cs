using Godot;
using System;

public partial class Arrow : Area2D
{
    [ExportGroup("Base Variables")]
    [Export]
    private float _speed = 200.0f;
    [Export]
    private float _damage = 20.0f;
    [Export]
    private float _slownessIntensity = 1.5f;
    [Export]
    private float _lifespan = 5.0f;

    public override void _Ready()
    {
        GetTree().CreateTimer(_lifespan).Timeout += QueueFree;
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += Transform.X * _speed * (float)delta;
    }

    public void _on_body_entered(Node2D body)
    {
        if(body is Player) return;
        if(body is IDamageable damageable) damageable.TakeDamage(_damage);
        QueueFree();
    }
}
