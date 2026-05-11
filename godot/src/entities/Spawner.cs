using Godot;
using System;

public partial class Spawner : Node2D
{
    [ExportGroup("Nodes")]
    [Export]
    public PackedScene EnemyPrefab { get; private set; }
    [Export]
    public Timer SpawnTimer { get; private set; }
    [Export]
    public int EnemiesToSpawn { get; private set; } = 30;
    [Export]
    public Label DisplayEnemiesLeft { get; private set; }
    [Export]
    public PointLight2D Light { get; private set; }

    private int _enemiesSpawned = 0;

    public override void _Ready()
    {
        DisplayEnemiesLeft.Text = EnemiesToSpawn.ToString();
    }

    public void _on_area_2d_body_entered(Node2D body)
    {
        if(body is Player)
        {
            SpawnTimer.Start();
            SpawnTimer.WaitTime = 3.0f;
        }
    }

    public void _on_spawn_timer_timeout()
    {
        if(_enemiesSpawned < EnemiesToSpawn)
        {
            SpawnEnemy();
        }
        else
        {
            SpawnTimer.Stop();
        }
    }

    private void SpawnEnemy()
    {
        int wave = GD.RandRange(1, 3);
        for(int i = 0; i < wave; i++)
        {
            _enemiesSpawned++;
            DisplayEnemiesLeft.Text = (EnemiesToSpawn - _enemiesSpawned).ToString();

            Enemy enemy = EnemyPrefab.Instantiate<Enemy>();
    
            GetTree().CurrentScene.AddChild(enemy);
    
            enemy.GlobalPosition = GlobalPosition + new Vector2((float)GD.RandRange(-30, 15), (float)GD.RandRange(-30, 0));
        }
        Light.Energy = 0.0f;
        Tween tween = CreateTween();
		tween.TweenProperty(Light, "energy", 5.0f, 0.125f)
            .SetTrans(Tween.TransitionType.Cubic)
			.SetEase(Tween.EaseType.Out);
        tween.Chain().TweenProperty(Light, "energy", 0.0f, 0.125f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.In);
    }
}
