using Godot;
using System;

public partial class HeavyAttackState : State
{
    public HeavyAttackState(Character character) : base(character) {}

    public override void Enter()
    {
        _character.AnimPlayer.Play(_character.AnimHeavyAttackName);
        _character.CurrentSpeedMultiplier = 0.1f;
    }

    public override void OnAnimationFinished(string animName)
    {
        if(animName == _character.AnimHeavyAttackName)
        {
            _character.AfterHeavyAttack();
            _character.ChangeState(new IdleState(_character));
        }
    }

    public override void Exit()
    {
        _character.CurrentSpeedMultiplier = 1.0f;
    }

}
