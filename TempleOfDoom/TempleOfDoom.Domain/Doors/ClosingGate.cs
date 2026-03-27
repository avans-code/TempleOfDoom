using TempleOfDoom.Domain.Models;

namespace TempleOfDoom.Domain.Doors;

public class ClosingGate : IDoor
{
    public bool IsOpen { get; private set; } = true;

    public bool CanEnter(Player player, Room currentRoom)
    {
        return IsOpen;
    }

    public void OnEnter()
    {
        IsOpen = false;
    }
}