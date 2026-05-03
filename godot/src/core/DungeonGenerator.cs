using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class DungeonGenerator : Node2D
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
        GenerateAsync();
    }

    private async void GenerateAsync()
    {
        Node2D startRoom = StartRooms.PickRandom().Instantiate<Node2D>();
        AddChild(startRoom);
        startRoom.GlobalPosition = Vector2.Zero;
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        AddExitsToQueue(startRoom);

        while(_exitsToProcess.Count > 0)
        {
            Marker2D currentExit = _exitsToProcess.Dequeue();
            PackedScene pickedRoom = _currentRoomCount < 30
                ? PickCompatibleRoom(MidRooms, currentExit)
                : PickCompatibleRoom(EndRooms, currentExit);

            if(pickedRoom != null) await SpawnRoomAsync(pickedRoom, currentExit);
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

    private async Task SpawnRoomAsync(PackedScene roomPrefab, Marker2D exit)
    {
        Room newRoom = roomPrefab.Instantiate<Room>();
        Marker2D entrance = newRoom.GetNode<Marker2D>("Entrance");
        Vector2 newPosition = exit.GlobalPosition - entrance.Position;

        AddChild(newRoom);
        newRoom.GlobalPosition = newPosition;
        
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        Area2D area = newRoom.GetNode<Area2D>("Area2D");
        var overlapping = area.GetOverlappingAreas();
        int overlapCount = overlapping.Count(a => a != area);

        if(overlapCount > 0)
        {
            newRoom.QueueFree();
            PackedScene endRoom = PickCompatibleRoom(EndRooms, exit);
            if(endRoom != null) await SpawnRoomAsync(endRoom, exit);
            return;
        }

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