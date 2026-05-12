using Godot;
using System;

public partial class Escape : Node2D
{
    [ExportGroup("Nodes")]
    [Export]
    public PackedScene ConfirmationScene { get; private set; }
    [Export]
    public Sprite2D PointerSprite { get; private set; }

    public static Vector2 CurrentEscapePosition { get; private set; }
    private float _time = 0.0f;
    private Vector2 _startPosition;

    public override void _Ready()
    {
        CurrentEscapePosition = GlobalPosition;
        if(PointerSprite != null)
        {
            _startPosition = PointerSprite.Position;
        }
    }

    public override void _Process(double delta)
    {
        if(PointerSprite != null)
        {
            _time += (float)delta;
            PointerSprite.Position = _startPosition + new Vector2(0, Mathf.Sin(_time * 2.0f) * 5.0f);
        }
    }

    public void _on_area_2d_body_entered(Node2D body)
    {
        if(body is Player)
        {
            // once again this exact export doesn't work and i have no idea why
            if(ConfirmationScene == null)
            {
                ConfirmationScene = GD.Load<PackedScene>("res://scenes/ui/EndRunConfirmation.tscn");
            }
            EndRunConfirmation confirmView = ConfirmationScene.Instantiate<EndRunConfirmation>();
            AddChild(confirmView);
        }
    }
}
