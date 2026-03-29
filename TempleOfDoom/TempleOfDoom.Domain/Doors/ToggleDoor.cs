using TempleOfDoom.Domain.Models;
using TempleOfDoom.Domain.Items;

namespace TempleOfDoom.Domain.Doors;

public class ToggleDoor : IDoor
{
    private Room? _currentRoom;

    public bool IsOpen => _currentRoom != null && CanEnter(null!, _currentRoom);

    public bool CanEnter(Player player, Room currentRoom)
    {
        _currentRoom = currentRoom; // Store reference for IsOpen property
        
        // Zoek de pressure plates in de kamer waar de deur bij hoort
        var plates = currentRoom.Entities.OfType<PressurePlate>().ToList();

        // Deur is 'open' (begaanbaar) als alle T's in de kamer ingedrukt zijn
        return !plates.Any() || plates.All(p => p.IsPressed);
    }

    public void OnEnter() { }
}