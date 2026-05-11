using Godot;
using System;

public partial class GameManager : Node2D
{
    private ColorRect _fadeRect;
    private CanvasLayer _layer;

    public override void _Ready()
    {
        _fadeRect = new ColorRect();
        _fadeRect.Color = Colors.Black;
        _fadeRect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
    
        _layer = new CanvasLayer();
        _layer.Layer = 100;
        AddChild(_layer);
        _layer.AddChild(_fadeRect);
    }

    public void _on_dungeon_generator_generation_completed()
    {
        FadeIn();
    }

    private void FadeIn()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(_fadeRect, "modulate:a", 0.0f, 0.8f)
              .SetTrans(Tween.TransitionType.Cubic);
        tween.Chain().TweenCallback(Callable.From(_layer.QueueFree));
    }
}
