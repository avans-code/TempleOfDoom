using TempleOfDoom.Domain.Models;

namespace TempleOfDoom.Domain.Doors;

public class ColoredDoor : IDoor
{
    public string Color { get; }
    public bool IsOpen { get; private set; } = false;

    public ColoredDoor(string color)
    {
        Color = color;
    }

    public bool CanEnter(Player player, Room currentRoom)
    {
        if (IsOpen) return true;
        
        if (player.HasKey(Color))
        {
            IsOpen = true;
            return true;
        }
        
        return false;
    }

    public void OnEnter() { }
}