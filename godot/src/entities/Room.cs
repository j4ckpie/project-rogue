using Godot;
using System;

public partial class Room : Node2D
{
    [ExportGroup("Basic Variables")]
    [Export]
    public int EntranceWidth { get; private set; }
    [Export]
    public int ExitWidth { get; private set; }
}
