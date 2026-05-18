using Godot;
using System;

public partial class EndRun : CanvasLayer
{
    [ExportGroup("Nodes")]
    [Export]
    public AnimationPlayer AnimPlayer { get; private set; }
    [Export]
    public Button YesButton { get; private set; }
    [Export]
    public Button NoButton { get; private set; }
    [Export]
    public Label TotalMeleeElims { get; private set; }
    [Export]
    public Label TotalXp { get; private set; }
    [Export]
    public Label TotalLvl { get; private set; }
    [Export]
    public Label TotalTimeSpent { get; private set; }
    [Export]
    public Label TotalScore { get; private set; }


    [ExportGroup("Basic Variables")]
    [Export]
    public string FadeInAnimName { get; private set; } = "fade_in";
    [Export]
    public string FadeOutAnimName { get; private set; } = "fade_out";
    [Export]
    public string ShowStatsAnimName { get; private set; } = "show_stats";

    private int _exitDecision;

    public override void _Ready()
    {
        Visible = true;
        AnimPlayer.Play(FadeInAnimName);
    }

    public void _on_animation_player_animation_finished(string animName)
    {
        if(animName.Equals(FadeInAnimName))
        {
            SetupStats();
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

    private void SetupStats()
    {
        float totalTime = Time.GetTicksMsec() / 1000.0f - Player.StartTime;
        int minutes = (int)totalTime / 60;
        int seconds = (int)totalTime % 60;

        int score = (int)((Player.TotalMeleeElims * 400.0f)
            + (Player.PlayerLvl * 200.0f)
            + (Player.TotalXp * 5.0f)
            + (10000.0f / totalTime * 100.0f));

        TotalMeleeElims.Text = Player.TotalMeleeElims.ToString();
        TotalXp.Text = ((int)Player.TotalXp).ToString();
        TotalLvl.Text = Player.PlayerLvl.ToString();
        TotalTimeSpent.Text = $"{minutes}m, {seconds}s";
        TotalScore.Text = score.ToString();

        AnimPlayer.Play(ShowStatsAnimName);
    }
}
