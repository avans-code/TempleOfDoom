using TempleOfDoom.Domain.Models;
using TempleOfDoom.Domain.Items;
using TempleOfDoom.Domain.Enemies;
using TempleOfDoom.Domain.Doors;

namespace TempleOfDoom.UI.Renderers;

public class ConsoleRenderer
{
    public void Render(Level level)
    {
        Console.Clear();
        Room room = level.CurrentRoom;

        for (int y = 0; y < room.Height; y++)
        {
            for (int x = 0; x < room.Width; x++)
            {
                if (level.Player.X == x && level.Player.Y == y)
                {
                    DrawChar('X', ConsoleColor.White);
                    continue;
                }

                if (TryDrawDoor(room, x, y, level))
                {
                    continue;
                }

                if (room.IsWall(x, y))
                {
                    DrawChar('#', ConsoleColor.DarkGray);
                    continue;
                }

                var entity = room.Entities.FirstOrDefault(e => e.X == x && e.Y == y);
                if (entity != null)
                {
                    DrawEntity(entity);
                }
                else
                {
                    // Two spaces for empty floor to maintain grid aspect ratio
                    Console.Write("  ");
                }
            }
            Console.WriteLine();
        }
        
        Console.WriteLine("\n--------------------------------------------------");
        Console.WriteLine($"Levens: {level.Player.Lives} | Stenen: {level.Player.StonesCollected}/5");
        Console.WriteLine("--------------------------------------------------");
    }

        private bool TryDrawDoor(Room room, int x, int y, Level level)
        {
            bool isDoor = false;
            char doorChar = ' '; // Default to space for open doors
            ConsoleColor doorColor = ConsoleColor.White;

            if (y == 0 && x == room.Width / 2 && room.OutgoingConnections.ContainsKey("NORTH")) { isDoor = true; }
            else if (y == room.Height - 1 && x == room.Width / 2 && room.OutgoingConnections.ContainsKey("SOUTH")) { isDoor = true; }
            else if (x == 0 && y == room.Height / 2 && room.OutgoingConnections.ContainsKey("WEST")) { isDoor = true; }
            else if (x == room.Width - 1 && y == room.Height / 2 && room.OutgoingConnections.ContainsKey("EAST")) { isDoor = true; }
        
            if (room.SpecialTiles.ContainsKey((x, y)) && room.SpecialTiles[(x, y)] == "innerdoor") { isDoor = true; }

            if (!isDoor) return false;

            Connection? conn = null;
            if (y == 0) conn = room.OutgoingConnections.GetValueOrDefault("NORTH");
            else if (y == room.Height - 1) conn = room.OutgoingConnections.GetValueOrDefault("SOUTH");
            else if (x == 0) conn = room.OutgoingConnections.GetValueOrDefault("WEST");
            else if (x == room.Width - 1) conn = room.OutgoingConnections.GetValueOrDefault("EAST");

            // Check if connection can be entered (door is open)
            bool canEnter = conn?.CanEnter(level.Player, room) ?? false;
            
if (!canEnter)
{
    // Door is closed, show appropriate symbol
    var doorType = conn?.Doors.FirstOrDefault();
    
    switch (doorType)
    {
        case ColoredDoor coloredDoor:
            doorChar = '=';
            doorColor = GetConsoleColor(coloredDoor.Color);
            break;
        case ClosingGate:
            doorChar = 'n';
            break;
        default:
            doorChar = '┴';
            break;
    }
}
            // If canEnter is true, doorChar remains ' ' (space)
        
            // Handle inner doors specifically
            if (room.SpecialTiles.ContainsKey((x, y)) && room.SpecialTiles[(x, y)] == "innerdoor") 
            { 
                var plates = room.Entities.OfType<PressurePlate>().ToList();
                if (plates.Any() && !plates.All(p => p.IsPressed))
                {
                    doorChar = '┴'; // Closed inner door
                }
                else
                {
                    doorChar = ' '; // Open inner door
                }
                doorColor = ConsoleColor.White;
            }

            DrawChar(doorChar, doorColor);
            return true;
        }

    private void DrawEntity(Entity entity)
    {
        switch (entity)
        {
            case EnemyAdapter:
                DrawChar('E', ConsoleColor.DarkRed);
                break;
            case SankaraStone:
                DrawChar('S', ConsoleColor.Yellow);
                break;
            case Key key:
                DrawChar('K', GetConsoleColor(key.Color));
                break;
            case Boobytrap trap:
                DrawChar(trap.IsDisappearing ? '@' : 'O', ConsoleColor.White);
                break;
            case PressurePlate:
                DrawChar('T', ConsoleColor.White);
                break;
            default:
                DrawChar('?', ConsoleColor.White);
                break;
        }
    }

    private void DrawChar(char c, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(c);
        Console.ResetColor();
        
        // Added space for aspect ratio correction
        Console.Write(" ");
    }

    private ConsoleColor GetConsoleColor(string colorString)
    {
        return colorString.ToLower() switch
        {
            "red" => ConsoleColor.Red,
            "green" => ConsoleColor.Green,
            "blue" => ConsoleColor.Blue,
            "yellow" => ConsoleColor.Yellow,
            _ => ConsoleColor.White
        };
    }
}