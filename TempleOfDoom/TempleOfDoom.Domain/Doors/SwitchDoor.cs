using TempleOfDoom.Domain.Models;

namespace TempleOfDoom.Domain.Doors;

public class SwitchDoor : IDoor
{
    public bool IsOpen { get; private set; }

    public void Unlock()
    {
        IsOpen = true; 
    }

    public bool CanEnter(Player player, Room currentRoom) => IsOpen;
    public void OnEnter() { }
}