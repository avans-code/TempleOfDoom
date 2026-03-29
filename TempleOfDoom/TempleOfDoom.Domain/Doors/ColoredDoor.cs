using TempleOfDoom.Domain.Models;

namespace TempleOfDoom.Domain.Doors;

public class ColoredDoor(string color) : IDoor
{
    public string Color { get; } = color;
    public bool IsOpen { get; private set; }

    public bool CanEnter(Player player, Room currentRoom)
    {
        if (IsOpen) return true;
        if (!player.HasKey(Color)) return false;
        IsOpen = true;
        return true;
    }

    public void OnEnter() { }
    public void Unlock() { } // Doet niks
}