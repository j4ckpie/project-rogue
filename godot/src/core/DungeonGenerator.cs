using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DungeonGenerator : Node
{
    [ExportGroup("Nodes")]
    [Export]
    public Godot.Collections.Array<PackedScene> StartRooms { get; private set; }
    [Export]
    public Godot.Collections.Array<PackedScene> MidRooms { get; private set; }
    [Export]
    public Godot.Collections.Array<PackedScene> EndRooms { get; private set; }

    private int _currentRoomCount = 0;
    private Queue<Marker2D> _exitsToProcess = new Queue<Marker2D>();

    public override void _Ready()
	{
		Generate();
	}

    private void Generate()
    {
        Node2D startRoom = StartRooms.PickRandom().Instantiate<Node2D>();
        AddChild(startRoom);
        startRoom.GlobalPosition = Vector2.Zero;

        AddExitsToQueue(startRoom);

        while(_exitsToProcess.Count > 0)
        {
            Marker2D currentExit = _exitsToProcess.Dequeue();
            if(_currentRoomCount < 2)
            {
                SpawnRoom(MidRooms.PickRandom(), currentExit);
            } 
            else
            {
                SpawnRoom(EndRooms.PickRandom(), currentExit);
            }
        }
    }

    private void SpawnRoom(PackedScene roomPrefab, Marker2D exit)
    {
        // TODO: Make more room variants and don't rotate them -> makes them look off.
        Node2D newRoom = roomPrefab.Instantiate<Node2D>();
        AddChild(newRoom);

        Marker2D entrance = newRoom.GetNode<Marker2D>("Entrance");
        newRoom.GlobalPosition = exit.GlobalPosition - entrance.Position;
        newRoom.GlobalRotationDegrees = exit.GlobalRotationDegrees;
        // ResetEntityRotation(newRoom);

        _currentRoomCount++;
        AddExitsToQueue(newRoom);
    }

    private void ResetEntityRotation(Node2D room)
    {
        foreach(Node child in room.GetChildren())
        {
            if(child is Character character) character.GlobalRotationDegrees = 0.0f;
        }
    }

    private void AddExitsToQueue(Node2D room)
    {
        foreach(Node child in room.GetChildren())
        {
            if(child is Marker2D marker && marker.Name.ToString().StartsWith("Exit"))
            {
                _exitsToProcess.Enqueue(marker);
            }
        }
    }
}
