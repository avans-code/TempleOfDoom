using TempleOfDoom.Domain.Doors;

namespace TempleOfDoom.Domain.Models;

public class Connection
{
    public Room TargetRoom { get; }
    public List<IDoor> Doors { get; } = new();

    public Connection(Room targetRoom)
    {
        TargetRoom = targetRoom;
    }

    // Check if all doors on this connection allow entry
    public bool CanEnter(Player player, Room currentRoom)
    {
        return Doors.All(d => d.CanEnter(player, currentRoom));
    }

    // Trigger behavior for all doors when passing through
    public void OnEnter()
    {
        foreach (var door in Doors)
        {
            door.OnEnter();
        }
    }
}