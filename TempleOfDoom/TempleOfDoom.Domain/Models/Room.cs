using TempleOfDoom.Domain.Enemies;
using TempleOfDoom.Domain.Items;

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
        // Check 1: Is het de buitenrand?
        if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1) return true;
        
        // Check 2: Is het een binnenmuur uit Module C?
        if (SpecialTiles.TryGetValue((x, y), out string tileType) && tileType == "wall") return true;

        return false;
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
    
    public void HandleShooting(Player player)
    {
        // De kamer weet waar de vijanden zijn, dus de kamer berekent de afstand
        var enemies = Entities.OfType<EnemyAdapter>().ToList();
        foreach (var enemy in enemies)
        {
            int dx = Math.Abs(enemy.X - player.X);
            int dy = Math.Abs(enemy.Y - player.Y);
            
            if ((dx == 1 && dy == 0) || (dx == 0 && dy == 1))
            {
                enemy.TakeDamage(1);
                if (enemy.IsDead) Entities.Remove(enemy);
            }
        }
    }

    public void MoveEnemies()
    {
        foreach (var enemy in Entities.OfType<EnemyAdapter>().ToList())
        {
            enemy.Move();
        }
    }

    public void ResolveCollisions(Player player)
    {
        if (Entities.OfType<EnemyAdapter>().Any(e => e.X == player.X && e.Y == player.Y))
        {
            player.TakeDamage(1);
        }
    }

    public void UpdatePressurePlates(Player player)
    {
        foreach (var plate in Entities.OfType<PressurePlate>())
        {
            plate.IsOccupied = (player.X == plate.X && player.Y == plate.Y) || 
                               Entities.OfType<EnemyAdapter>().Any(e => e.X == plate.X && e.Y == plate.Y);
        }
    }

    public void HandleItemPickup(Player player)
    {
        var itemOnFloor = Entities.OfType<Item>().FirstOrDefault(i => i.X == player.X && i.Y == player.Y);
        itemOnFloor?.Interact(player, this); // Polymorfisme
    }
    
    public void PlacePlayerAtEntrance(Player player, string entryDirection)
    {
        if (entryDirection == "NORTH") { player.Y = Height - 1; player.X = Width / 2; }
        if (entryDirection == "SOUTH") { player.Y = 0; player.X = Width / 2; }
        if (entryDirection == "WEST") { player.X = Width - 1; player.Y = Height / 2; }
        if (entryDirection == "EAST") { player.X = 0; player.Y = Height / 2; }
    }
}