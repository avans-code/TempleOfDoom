namespace TempleOfDoom.Domain.Models;

public abstract class Entity
{
    public int X { get; set; }
    public int Y { get; set; }

    protected Entity(int x, int y)
    {
        X = x;
        Y = y;
    }
}