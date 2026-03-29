using TempleOfDoom.Domain.Models;

namespace TempleOfDoom.Domain.Doors;

public class OpenOnOddDoor : IDoor
{
    private readonly Level _level;

    public OpenOnOddDoor(Level level)
    {
        _level = level;
    }

    public bool IsOpen => _level.Player.Lives % 2 != 0;

    public bool CanEnter(Player player, Room room)
    {
        return player.Lives % 2 != 0; 
    }

    public void OnEnter() { }
    public void Unlock() { } // Doet niks (tenzij je deze als échte decorator maakt met _innerDoor.Unlock(), maar voor nu is leeg prima om de errors te fixen)
}