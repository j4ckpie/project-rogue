using Godot;
using System;

public partial class Explosion : Node2D
{
    [ExportGroup("Nodes")]
    [Export]
    public CpuParticles2D Particles { get; private set; }
    [Export]
    public Timer KillTimer { get; private set; }

    public override void _Ready()
    {
        Particles.OneShot = true;
        Particles.Emitting = true;
    }

    public void _on_kill_timer_timeout()
    {
        QueueFree();
    }
}
