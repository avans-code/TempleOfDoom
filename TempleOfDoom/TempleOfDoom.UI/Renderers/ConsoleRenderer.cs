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
        Connection? conn = null;
        bool isInnerDoor = room.SpecialTiles.TryGetValue((x, y), out string tileType) && tileType == "innerdoor";

        if (room.IsEdgeDoor(x, y, out string direction))
        {
            conn = room.OutgoingConnections.GetValueOrDefault(direction);
        }
        else if (isInnerDoor)
        {
            conn = room.OutgoingConnections.GetValueOrDefault("INNER");
        }

        if (conn == null) return false;

        if (conn.CanEnter(level.Player, room))
        {
            DrawChar(' ', ConsoleColor.White);
            return true;
        }

        char doorChar = conn.IsHorizontal ? '=' : '|';
        ConsoleColor doorColor = ConsoleColor.White;

        if (conn.Doors.OfType<ClosingGate>().Any()) doorChar = 'n';
        if (conn.Doors.OfType<ToggleDoor>().Any() || conn.Doors.OfType<SwitchDoor>().Any()) doorChar = '┴';

        var coloredDoor = conn.Doors.OfType<ColoredDoor>().FirstOrDefault();
        if (coloredDoor != null)
        {
            doorColor = GetConsoleColor(coloredDoor.Color);
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