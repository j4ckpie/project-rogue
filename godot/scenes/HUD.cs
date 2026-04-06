using Godot;
using System;

public partial class HUD : CanvasLayer
{
    [ExportGroup("Nodes")]
    [Export]
	private TextureProgressBar _healthBar;
    [Export]
	private TextureProgressBar _damageBar;
	[Export]
	private TextureProgressBar _xpBar;
    [Export]
	private TextureProgressBar _newXpBar;

    private Tween _damageTween;
    private Tween _newXpTween;

    public void _on_player_hp_changed(float amount)
    {
        _healthBar.Value = amount;
        PlayFillProgressBarAnimation(_damageTween, _damageBar, amount, 0.5f, 0.5f);
    }

    public void _on_player_xp_changed(float amount)
    {
        _newXpBar.Value = amount;
        PlayFillProgressBarAnimation(_newXpTween, _xpBar, amount, 0.25f, 0.25f);
    }

    private void PlayFillProgressBarAnimation(Tween tween, Node target, float amount, float duration, float pause)
    {
        // if animation is already running, start over
        if(tween != null && tween.IsRunning())
        {
            tween.Kill();
        }

        // play animation
        tween = CreateTween();
        tween.TweenInterval(pause);
        tween.TweenProperty(target, "value", amount, duration)
                    .SetTrans(Tween.TransitionType.Sine)
                    .SetEase(Tween.EaseType.Out);
    }
}
