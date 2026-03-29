using TempleOfDoom.Domain.Models;

namespace TempleOfDoom.Domain.Items;

public class Key(int x, int y, string color) : Item(x, y)
{
    public string Color { get; } = color;

    public override void Interact(Player player, Room room)
    {
        player.AddKey(Color);
        room.Entities.Remove(this); // Verijder sleutel
    }
}