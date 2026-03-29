using TempleOfDoom.Domain.Models;

namespace TempleOfDoom.Domain.Items;

public abstract class Item(int x, int y) : Entity(x, y)
{
    public abstract void Interact(Player player, Room room);
}