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
            PackedScene pickedRoom = _currentRoomCount < 2
                ? PickCompatibleRoom(MidRooms, currentExit)
                : PickCompatibleRoom(EndRooms, currentExit);

            //if(pickedRoom == null && _currentRoomCount < 2) pickedRoom = PickCompatibleRoom(EndRooms, currentExit);
            if(pickedRoom != null) SpawnRoom(pickedRoom, currentExit);
        }
    }

    private PackedScene PickCompatibleRoom(Godot.Collections.Array<PackedScene> rooms, Marker2D exit)
    {
        var shuffled = rooms.OrderBy(_ => GD.Randf()).ToList();
        foreach(PackedScene room in shuffled)
        {
            if(CheckRoomCompatibility(room, exit)) return room;
        }
        return null;
}

    private bool CheckRoomCompatibility(PackedScene roomPrefab, Marker2D exit)
    {
        Room newRoom = roomPrefab.Instantiate<Room>();
        Marker2D entrance = newRoom.GetNode<Marker2D>("Entrance");

        bool isCompatible = entrance.RotationDegrees == exit.RotationDegrees &&
            entrance.GetParent<Room>().EntranceWidth == exit.GetParent<Room>().ExitWidth;
        newRoom.Free();
        
        return isCompatible;
    }

    private void SpawnRoom(PackedScene roomPrefab, Marker2D exit)
    {
        Room newRoom = roomPrefab.Instantiate<Room>();
        AddChild(newRoom);

        GD.Print("seima");

        Marker2D entrance = newRoom.GetNode<Marker2D>("Entrance");
        newRoom.GlobalPosition = exit.GlobalPosition - entrance.Position;

        _currentRoomCount++;
        AddExitsToQueue(newRoom);
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
