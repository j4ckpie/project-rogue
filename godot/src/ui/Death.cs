using Godot;
using System;

public partial class Death : CanvasLayer
{
    [ExportGroup("Nodes")]
    [Export]
    public AnimationPlayer AnimPlayer { get; private set; }
    [Export]
    public Button YesButton { get; private set; }
    [Export]
    public Button NoButton { get; private set; }

    [ExportGroup("Basic Variables")]
    [Export]
    public string FadeInAnimName { get; private set; } = "fade_in";
    [Export]
    public string FadeOutAnimName { get; private set; } = "fade_out";
    [Export]
    public string ShowAnimName { get; private set; } = "show";

    private int _exitDecision;
    
    public override void _Ready()
    {
        Visible = true;
        AnimPlayer.Play(FadeInAnimName);
        GetTree().Paused = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    public void _on_animation_player_animation_finished(string animName)
    {
        if(animName.Equals(FadeInAnimName))
        {
            AnimPlayer.Play(ShowAnimName);
        }
        else if(animName.Equals(FadeOutAnimName))
        {
            switch(_exitDecision)
            {
                default: break;
                case 0: GetTree().Quit(); break;
                case 1: GetTree().Paused = false; GetTree().ReloadCurrentScene(); break;
            }
        }
    }

    public void _on_yes_button_mouse_entered()
    {
        YesButton.Text = ">   Yes!   <";
    }

    public void _on_yes_button_mouse_exited()
    {
        YesButton.Text = "Yes!";
    }

    public void _on_yes_button_pressed()
    {
        _exitDecision = 1;
        AnimPlayer.Play(FadeOutAnimName);
    }

    public void _on_no_button_mouse_entered()
    {
        NoButton.Text = ">   No...   <";
    }

    public void _on_no_button_mouse_exited()
    {
        NoButton.Text = "No...";
    }

    public void _on_no_button_pressed()
    {
        _exitDecision = 0;
        AnimPlayer.Play(FadeOutAnimName);
    }
}
