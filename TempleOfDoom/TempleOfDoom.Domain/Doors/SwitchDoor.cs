using System.Linq;
using TempleOfDoom.Domain.Models;
using TempleOfDoom.Domain.Items;

namespace TempleOfDoom.Domain.Doors;

public class SwitchDoor : IDoor
{
    public bool IsOpen { get; private set; } = false;

    public bool CanEnter(Player player, Room currentRoom)
    {
        if (IsOpen) return true;

        var pressurePlates = currentRoom.Entities.OfType<PressurePlate>().ToList();
        
        if (pressurePlates.Any() && pressurePlates.All(p => p.IsPressed))
        {
            IsOpen = true;
            return true;
        }
        
        return false;
    }

    public void OnEnter() { }
}