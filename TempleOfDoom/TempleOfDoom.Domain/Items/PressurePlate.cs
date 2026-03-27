namespace TempleOfDoom.Domain.Items;

public class PressurePlate : Item
{
    public bool IsPressed { get; set; } = false;

    public PressurePlate(int x, int y) : base(x, y)
    {
    }
}