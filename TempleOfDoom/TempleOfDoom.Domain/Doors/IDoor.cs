using TempleOfDoom.Domain.Models;

namespace TempleOfDoom.Domain.Doors;

public interface IDoor
{
    bool IsOpen { get; }
    bool CanEnter(Player player, Room currentRoom);
    void OnEnter();
}