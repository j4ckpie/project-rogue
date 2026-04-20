using Godot;
using System;

public partial class AttackState : State
{
    public AttackState(Character character) : base(character) {}

    public override void Enter()
    {
        _character.AnimPlayer.Play(_character.AnimAttackName);
    }

    public override void OnAnimationFinished(string animName)
    {
        if(animName == _character.AnimAttackName)
        {
            _character.AfterAttack();
            _character.ChangeState(new IdleState(_character));
        }
    }
}
