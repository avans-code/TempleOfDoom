using TempleOfDoom.Domain.Models;

namespace TempleOfDoom.Domain.Doors;

public interface IDoor
{
    bool CanEnter(Player player, Room currentRoom);
    void OnEnter();
}