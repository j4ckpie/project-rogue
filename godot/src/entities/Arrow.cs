using Godot;
using System;

public partial class Arrow : Area2D
{
    [ExportGroup("Base Variables")]
    [Export]
    public float Speed { get; private set; } = 200.0f;
    [Export]
    public float Damage { get; private set; } = 20.0f;
    [Export]
    public float SlownessIntensity { get; private set; } = 1.7f;
    
    private float _lifespan = 5.0f;

    public override void _Ready()
    {
        GetTree().CreateTimer(_lifespan).Timeout += QueueFree;
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += Transform.X * Speed * (float)delta;
    }

    public void _on_body_entered(Node2D body)
    {
        if(body is Player) return;
        if(body is IDamageable damageable)
        {
            damageable.TakeDamage(Damage);
            damageable.ApplySlowness(1.0f / (1.0f + SlownessIntensity));
        }
        QueueFree();
    }
}
