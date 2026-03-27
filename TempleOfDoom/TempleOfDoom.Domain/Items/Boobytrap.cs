namespace TempleOfDoom.Domain.Items;

public class Boobytrap : Item
{
    public int Damage { get; }
    public bool IsDisappearing { get; }
    public bool HasTriggered { get; set; } = false;

    public Boobytrap(int x, int y, int damage, bool isDisappearing) : base(x, y)
    {
        Damage = damage;
        IsDisappearing = isDisappearing;
    }
}