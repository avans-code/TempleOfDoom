namespace TempleOfDoom.Domain.Models;

public class Room
{
    public int Id { get; }
    public int Width { get; }
    public int Height { get; }
    
    // Voor Module C: Lijst van vijanden (via Adapter dadelijk) en binnenmuren
    public List<Entity> Entities { get; } = new(); 
    public Dictionary<(int x, int y), string> SpecialTiles { get; } = new();
    public Dictionary<string, Connection> OutgoingConnections { get; } = new();

    public Room(int id, int width, int height)
    {
        Id = id;
        Width = width;
        Height = height;
    }

    public bool IsWall(int x, int y)
    {
        // Check buitenmuren
        if (x <= 0 || x >= Width - 1 || y <= 0 || y >= Height - 1) return true;
        
        // Check binnenmuren (Module C)
        return SpecialTiles.ContainsKey((x, y)) && SpecialTiles[(x, y)] == "wall";
    }
}