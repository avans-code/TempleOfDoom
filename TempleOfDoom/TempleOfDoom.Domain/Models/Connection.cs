namespace TempleOfDoom.Domain.Models;

public class Connection
{
    public Room TargetRoom { get; }
    public List<Doors.IDoor> Doors { get; } = new();
    public bool IsHorizontal { get; set; }

    public Connection(Room targetRoom)
    {
        TargetRoom = targetRoom;
    }

    public bool CanEnter(Player player, Room currentRoom)
    {
        return Doors.All(d => d.CanEnter(player, currentRoom));
    }

    public void OnEnter()
    {
        foreach (var door in Doors) door.OnEnter();
    }
}