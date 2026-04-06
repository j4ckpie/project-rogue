using Godot;
using System;

public partial class HUD : CanvasLayer
{
    [ExportGroup("Nodes")]
    [Export]
	private TextureProgressBar _healthBar;
	[Export]
	private TextureProgressBar _xpBar;

    public void _on_player_hp_changed(float amount)
    {
        _healthBar.Value = amount;
    }

    public void _on_player_xp_changed(float amount)
    {
        _xpBar.Value = amount;
        GD.Print("HELLO");
    }
}
