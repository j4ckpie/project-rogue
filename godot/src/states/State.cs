using Godot;
using System;

public abstract partial class State : Node
{
    protected Character _character;

    public State(Character character)
    {
        _character = character;
    }

    public virtual void Enter() {}
    public virtual void PhysicsProcess(double delta) {}
    public virtual void Exit() {}
    public virtual void OnAnimationFinished(string animName) {}
}
