using Godot;
using System;

public partial class DeathState : State
{
    public DeathState(Character character) : base(character) {}

    public override void Enter()
    {
        _character.AnimPlayer.Play(_character.AnimDeathName);
        _character.Velocity = Vector2.Zero;
        _character.IsDead = true;

        _character.SetDeferred(CollisionObject2D.PropertyName.CollisionLayer, 0u);
        _character.SetDeferred(CollisionObject2D.PropertyName.CollisionMask, 0u);
    }

    public override void OnAnimationFinished(string animName)
    {
        if(animName == _character.AnimDeathName)
        {
            _character.PlayFadeAnimation(_character.TargetSprite, 0.0f, 2.0f);
        }
    }
}
