using TempleOfDoom.Domain.Models;

namespace TempleOfDoom.Domain.Items;

public abstract class Item : Entity
{
    protected Item(int x, int y) : base(x, y)
    {
    }
}