using Godot;
using System;

public partial class PopUp : Marker2D
{
	[Export]
	public Label Label { get; private set; }
	[Export]
	public PointLight2D Light { get; private set; }

	public void Start(float amount, string pre, string post)
	{
		Label.Text = $"{pre}{Mathf.Round(amount)} {post}";

		Vector2 finalPos = Position + new Vector2((float)GD.RandRange(-15, 15), (float)GD.RandRange(-15, -5));
		Tween tween = CreateTween();
		tween.SetParallel(true);
		tween.TweenProperty(Light, "texture_scale", 1.2f, 0.1f);
		tween.TweenProperty(Light, "energy", 5.0f, 0.1f);
		
		tween.Chain().SetParallel(true);

		tween.TweenProperty(this, "position", finalPos, 0.75f)
			 .SetTrans(Tween.TransitionType.Cubic)
			 .SetEase(Tween.EaseType.Out);

		tween.TweenProperty(this, "modulate:a", 0.0f, 0.75f);
		tween.TweenProperty(Light, "energy", 0.0f, 0.75f);
		tween.TweenProperty(Light, "texture_scale", 0.5f, 0.75f);

		tween.Chain().TweenCallback(Callable.From(QueueFree));
	}
}
