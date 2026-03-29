namespace TempleOfDoom.Domain.Models;

public class Room
{
    public int Id { get; }
    public int Width { get; }
    public int Height { get; }

    public List<Entity> Entities { get; } = new();
    public Dictionary<string, Connection> OutgoingConnections { get; } = new();
    public Dictionary<(int x, int y), string> SpecialTiles { get; } = new();

    public Room(int id, int width, int height)
    {
        Id = id;
        Width = width;
        Height = height;
    }

    public bool IsWall(int x, int y)
    {
        return x == 0 || y == 0 || x == Width - 1 || y == Height - 1;
    }

    public bool IsEdgeDoor(int x, int y, out string direction)
    {
        direction = "";
        if (y == 0 && x == Width / 2) { direction = "NORTH"; return true; }
        if (y == Height - 1 && x == Width / 2) { direction = "SOUTH"; return true; }
        if (x == 0 && y == Height / 2) { direction = "WEST"; return true; }
        if (x == Width - 1 && y == Height / 2) { direction = "EAST"; return true; }
        return false;
    }

    public string GetExitDirection(int targetX, int targetY)
    {
        if (targetY < 0 && targetX == Width / 2) return "NORTH";
        if (targetY >= Height && targetX == Width / 2) return "SOUTH";
        if (targetX < 0 && targetY == Height / 2) return "WEST";
        if (targetX >= Width && targetY == Height / 2) return "EAST";
        return string.Empty;
    }
}