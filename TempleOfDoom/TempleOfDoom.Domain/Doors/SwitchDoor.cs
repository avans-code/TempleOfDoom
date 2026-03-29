using TempleOfDoom.Domain.Models;

namespace TempleOfDoom.Domain.Doors;

public class SwitchDoor : IDoor
{
    public bool IsOpen { get; private set; } = false;

    public void Unlock()
    {
        IsOpen = true;
    }

    public bool CanEnter(Player player, Room currentRoom)
    {
        return IsOpen;
    }

    public void OnEnter() { }
}