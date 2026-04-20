using Godot;
using System;

public partial class MoveState : State
{
    public MoveState(Character character) : base(character) {}

    public override void Enter()
    {
        _character.AnimPlayer.Play(_character.AnimWalkName);
    }

    public override void PhysicsProcess(double delta)
    {
        if(_character.Direction.Length() == 0)
        {
            _character.ChangeState(new IdleState(_character));
            return;
        }

        if(_character.IsSprinting && !_character.IsSneaking)
        {
            _character.CurrentSpeedMultiplier = _character.SprintSpeedMultiplier;
            _character.AnimPlayer.SpeedScale = 1.075f;
        }
        else if(!_character.IsSprinting && _character.IsSneaking)
        {
            _character.CurrentSpeedMultiplier = _character.SneakSpeedMultiplier;
            _character.AnimPlayer.SpeedScale = 0.5f;
        }
        else
        {
            _character.CurrentSpeedMultiplier = 1.0f;
            _character.AnimPlayer.SpeedScale = 1.0f;
        }
    }

    public override void Exit()
    {
        _character.CurrentSpeedMultiplier = 1.0f;
        _character.AnimPlayer.SpeedScale = 1.0f;
    }
}
