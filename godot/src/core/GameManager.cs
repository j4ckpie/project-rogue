using Godot;
using System;

public partial class GameManager : Node2D
{
    [ExportGroup("Nodes")]
    [Export]
    public PackedScene PauseScene { get; private set; }

    [ExportGroup("Basic Variables")]
    [Export]
    public string ActionPauseName { get; private set; } = "pause";

    private ColorRect _fadeRect;
    private CanvasLayer _layer;
    private CanvasLayer _pauseView;

    public override void _Ready()
    {
        CreateCanvasLayer();
        
        if(PauseScene == null)
        {
            PauseScene = GD.Load<PackedScene>("res://scenes/ui/Pause.tscn");
        }
    }

    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed(ActionPauseName)) TogglePause();
    }

    public void _on_dungeon_generator_generation_completed()
    {
        FadeIn();
    }

    private void CreateCanvasLayer()
    {
        _fadeRect = new ColorRect();
        _fadeRect.Color = Colors.Black;
        _fadeRect.SetAnchorsPreset(Control.LayoutPreset.FullRect);
    
        _layer = new CanvasLayer();
        _layer.Layer = 100;
        AddChild(_layer);
        _layer.AddChild(_fadeRect);
    }

    private void FadeIn()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(_fadeRect, "modulate:a", 0.0f, 0.8f)
              .SetTrans(Tween.TransitionType.Cubic);
        tween.Chain().TweenCallback(Callable.From(_layer.QueueFree));
    }

    private void TogglePause()
    {
        if(!GetTree().Paused)
        {
            GetTree().Paused = true;
            _pauseView = PauseScene.Instantiate<CanvasLayer>();
            AddChild(_pauseView);
        }
        else
        {
            GetTree().Paused = false;
            _pauseView.QueueFree();
        }
    }
}
