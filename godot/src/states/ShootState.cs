using Godot;
using System;

public partial class ShootState : State
{
    public ShootState(Character character) : base(character) {}

    public override void Enter()
    {
        _character.AnimPlayer.Play(_character.AnimShootName);
        _character.CurrentSpeedMultiplier = 0.25f;
    }

    public override void OnAnimationFinished(string animName)
    {
        if(animName == _character.AnimShootName)
        {
            _character.AfterShoot();
            _character.ChangeState(new IdleState(_character));
        }
    }

    public override void Exit()
    {
        _character.CurrentSpeedMultiplier = 1.0f;
    }
}
