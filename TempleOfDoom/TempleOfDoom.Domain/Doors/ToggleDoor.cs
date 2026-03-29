using TempleOfDoom.Domain.Models;
using TempleOfDoom.Domain.Items;

namespace TempleOfDoom.Domain.Doors;

public class ToggleDoor : IDoor
{
    private bool IsOpen { get; set; }

    public bool CanEnter(Player player, Room currentRoom)
    {
        if (IsOpen) return true;
        var plates = currentRoom.Entities.OfType<PressurePlate>().ToList();

        if (!plates.Any() || plates.All(p => p.IsPressed))
        {
            IsOpen = true;
            return true;
        }
        return false;
    }

    public void OnEnter() { }
    public void Unlock() { } // Doet niks
}