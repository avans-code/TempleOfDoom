using TempleOfDoom.Domain.Models;

namespace TempleOfDoom.Domain.Items;

public class Boobytrap(int x, int y, int damage, bool isDisappearing) : Item(x, y)
{
    public int Damage { get; } = damage;
    public bool IsDisappearing { get; } = isDisappearing;
    public bool HasTriggered { get; set; }

    public override void Interact(Player player, Room room)
    {
        if (!HasTriggered)
        {
            player.TakeDamage(Damage);
            HasTriggered = true;
            if (IsDisappearing)
            {
                room.Entities.Remove(this);
            }
        }
    }
}