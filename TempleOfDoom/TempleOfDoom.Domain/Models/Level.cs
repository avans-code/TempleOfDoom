namespace TempleOfDoom.Domain.Models;

public class Level
{
    public Dictionary<int, Room> Rooms { get; } = new();
    public Player Player { get; set; }
    public int CurrentRoomId { get; set; }
    public int StepCount { get; set; } = 0;
    public Room CurrentRoom => Rooms[CurrentRoomId];
}