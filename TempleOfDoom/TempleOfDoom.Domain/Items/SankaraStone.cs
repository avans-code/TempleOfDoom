using TempleOfDoom.Domain.Models;

namespace TempleOfDoom.Domain.Items;

public class SankaraStone(int x, int y) : Item(x, y)
{
    public override void Interact(Player player, Room room)
    {
        player.StonesCollected++;
        room.Entities.Remove(this); // Verwijder steen
    }
}