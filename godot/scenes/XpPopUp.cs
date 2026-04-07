using Godot;
using System;

public partial class XpPopUp : Marker2D
{
    [Export]
    private Label _label;
    [Export]
    private PointLight2D _light;

    public void Start(float amount)
    {
        _label.Text = $"+{Mathf.Round(amount)} XP";

        Vector2 finalPos = Position + new Vector2((float)GD.RandRange(-15, 15), (float)GD.RandRange(-15, -5));
        Tween tween = CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(_light, "texture_scale", 1.2f, 0.1f);
        tween.TweenProperty(_light, "energy", 5.0f, 0.1f);
        
        tween.Chain().SetParallel(true);

        tween.TweenProperty(this, "position", finalPos, 0.75f)
             .SetTrans(Tween.TransitionType.Cubic)
             .SetEase(Tween.EaseType.Out);

        tween.TweenProperty(this, "modulate:a", 0.0f, 0.75f);
        tween.TweenProperty(_light, "energy", 0.0f, 0.75f);
        tween.TweenProperty(_light, "texture_scale", 0.5f, 0.75f);

        tween.Chain().TweenCallback(Callable.From(QueueFree));
    }
}
