using Godot;
using System;

public partial class HUD : CanvasLayer
{
    [ExportGroup("Nodes")]
    [Export]
	public TextureProgressBar HealthBar { get; private set; }
    [Export]
    public TextureProgressBar DamageBar { get; private set; }
	[Export]
	public TextureProgressBar XpBar { get; private set; }
    [Export]
	public TextureProgressBar NewXpBar { get; private set; }
    [Export]
	public Label DisplayedLvl { get; private set; }

    private Tween _damageTween;
    private Tween _newXpTween;

    public void _on_player_hp_changed(float amount)
    {
        HealthBar.Value = amount;
        PlayFillProgressBarAnimation(_damageTween, DamageBar, amount, 0.5f, 0.5f);
    }

    public void _on_player_xp_changed(float amount)
    {
        NewXpBar.Value = amount;
        PlayFillProgressBarAnimation(_newXpTween, XpBar, amount, 0.5f, 0.5f);
    }

    public void _on_player_leveled_up(int amount)
    {
        DisplayedLvl.Text = amount.ToString();
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
