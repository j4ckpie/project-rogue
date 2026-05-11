using Godot;
using System;

public partial class Room : Node2D
{
    [ExportGroup("Nodes")]
    [Export]
    public Godot.Collections.Array<PackedScene> PossibleEnemies { get; private set; }
    [Export]
    public Godot.Collections.Array<Marker2D> PossiblePositions { get; private set; }

    [ExportGroup("Basic Variables")]
    [Export]
    public int EntranceWidth { get; private set; }
    [Export]
    public int ExitWidth { get; private set; }

    public override void _Ready()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        foreach(Marker2D loc in PossiblePositions)
        {
            if(GD.Randf() > 0.5)
            {
                Enemy enemy = PossibleEnemies.PickRandom().Instantiate<Enemy>();
                enemy.Position = loc.Position;
                AddChild(enemy);
            }
        }
    }
}
