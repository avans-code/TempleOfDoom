namespace TempleOfDoom.Domain.Items;

public class Key : Item
{
    public string Color { get; }

    public Key(int x, int y, string color) : base(x, y)
    {
        Color = color;
    }
}