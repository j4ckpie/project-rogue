using Godot;
using System;

public partial class EndRunConfirmation : CanvasLayer
{
    [ExportGroup("Nodes")]
    [Export]
    public PackedScene EndScene { get; private set; }
    [Export]
    public Button YesButton { get; private set; }
    [Export]
    public Button NoButton { get; private set; }

    public override void _Ready()
    {
        Visible = true;
        GetTree().Paused = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    public void _on_yes_button_mouse_entered()
    {
        YesButton.Text = ">    Yes...    <";
    }

    public void _on_yes_button_mouse_exited()
    {
        YesButton.Text = "Yes...";
    }

    public void _on_yes_button_pressed()
    {
        EndRun endRunView = EndScene.Instantiate<EndRun>();
        AddChild(endRunView);
    }

    public void _on_no_button_mouse_entered()
    {
        NoButton.Text = ">    No!    <";
    }

    public void _on_no_button_mouse_exited()
    {
        NoButton.Text = "No!";
    }

    public void _on_no_button_pressed()
    {
        GetTree().Paused = false;
        Input.MouseMode = Input.MouseModeEnum.Hidden;
        QueueFree();
    }
}
