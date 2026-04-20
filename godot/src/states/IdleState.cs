using Godot;
using System;

public partial class IdleState : State
{
    public IdleState(Character character) : base(character) {}

    public override void Enter()
    {
        _character.AnimPlayer.Play(_character.AnimIdleName);
        _character.CurrentSpeedMultiplier = 1.0f;
    }

    public override void PhysicsProcess(double delta)
    {
        if(_character.Direction.Length() > 0)
        {
            _character.ChangeState(new MoveState(_character));
        }
    }
}
