using Godot;
using System;

public partial class EndRun : CanvasLayer
{
    [ExportGroup("Nodes")]
    [Export]
    public AnimationPlayer AnimPlayer { get; private set; }
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
    public string ShowStatsAnimName { get; private set; } = "show_stats";

    public override void _Ready()
    {
        Visible = true;
        AnimPlayer.Play(FadeInAnimName);
    }

    public void _on_animation_player_animation_finished(string animName)
    {
        if(animName == FadeInAnimName)
        {
            SetupStats();
        }
    }

    private void SetupStats()
    {
        float totalTime = Time.GetTicksMsec() / 1000.0f - Player.StartTime;
        int minutes = (int)totalTime / 60;
        int seconds = (int)totalTime % 60;

        float score = (Player.TotalMeleeElims * 400.0f)
            + (Player.Lvl * 200.0f)
            + (Player.TotalXp * 5.0f)
            + (10000.0f / totalTime * 100.0f);

        TotalMeleeElims.Text = Player.TotalMeleeElims.ToString();
        TotalXp.Text = ((int)Player.TotalXp).ToString();
        TotalLvl.Text = Player.Lvl.ToString();
        TotalTimeSpent.Text = $"{minutes}m, {seconds}s";
        TotalScore.Text = score.ToString();

        AnimPlayer.Play(ShowStatsAnimName);
    }
}
