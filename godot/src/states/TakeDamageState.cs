using Godot;
using System;

public partial class TakeDamageState : State
{
    public TakeDamageState(Character character) : base(character) {}

    public override void Enter()
    {
        _character.AnimPlayer.Play(_character.AnimTakeDamageName);
        _character.CurrentSpeedMultiplier = 0.8f;
    }

    public override void OnAnimationFinished(string animName)
    {
        if(animName == _character.AnimTakeDamageName)
        {
            _character.ChangeState(new IdleState(_character));
        }
    }

    public override void Exit()
    {
        _character.CurrentSpeedMultiplier = 1.0f;
    }
}
