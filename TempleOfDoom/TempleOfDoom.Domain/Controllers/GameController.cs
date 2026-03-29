using TempleOfDoom.Domain.Models;
using TempleOfDoom.Domain.Items;
using TempleOfDoom.Domain.Doors;

namespace TempleOfDoom.Domain.Controllers;

public class GameController
{
    private readonly Level _level;

    public GameController(Level level)
    {
        _level = level;
        WireObservers();
    }

    public Level CurrentLevel => _level;
    public bool IsGameOver => _level.Player.Lives <= 0 || _level.Player.StonesCollected >= 5;

    private void WireObservers()
    {
        foreach (var room in _level.Rooms.Values)
        {
            var plates = room.Entities.OfType<PressurePlate>().ToList();
            if (plates.Count == 0) continue;

            foreach (var plate in plates)
            {
                plate.OnOccupancyChanged += () =>
                {
                    if (plates.All(p => p.IsOccupied))
                    {
                        foreach (var connection in room.OutgoingConnections.Values)
                        {
                            foreach (var door in connection.Doors)
                            {
                                door.Unlock(); // Polymorfisme lost Inappropriate Intimacy op
                            }
                        }
                    }
                };
            }
        }
    }

    public void HandleInput(ConsoleKey key)
    {
        _level.StepCount++;
        int dx = 0, dy = 0;

        switch (key)
        {
            case ConsoleKey.UpArrow: dy = -1; break;
            case ConsoleKey.DownArrow: dy = 1; break;
            case ConsoleKey.LeftArrow: dx = -1; break;
            case ConsoleKey.RightArrow: dx = 1; break;
            case ConsoleKey.Spacebar:
                _level.CurrentRoom.HandleShooting(_level.Player);
                ProcessRoomEvents();
                return; 
        }

        MovePlayer(dx, dy);
    }

    private void MovePlayer(int dx, int dy)
    {
        int targetX = _level.Player.X + dx;
        int targetY = _level.Player.Y + dy;
        Room currentRoom = _level.CurrentRoom;

        string exitDirection = currentRoom.GetExitDirection(targetX, targetY);

        if (!string.IsNullOrEmpty(exitDirection))
        {
            TryTransitionRoom(exitDirection);
        }
        else if (CanMoveTo(targetX, targetY))
        {
            _level.Player.X = targetX;
            _level.Player.Y = targetY;
            currentRoom.HandleItemPickup(_level.Player);
        }

        ProcessRoomEvents();
    }

    private void ProcessRoomEvents()
    {
        // De controller stuurt puur aan (Delegatie), de Room doet het werk (Information Expert)
        _level.CurrentRoom.MoveEnemies();
        _level.CurrentRoom.ResolveCollisions(_level.Player);
        _level.CurrentRoom.UpdatePressurePlates(_level.Player);
    }

    private void TryTransitionRoom(string direction)
    {
        Room currentRoom = _level.CurrentRoom;

        if (currentRoom.OutgoingConnections.TryGetValue(direction, out var connection))
        {
            if (connection.CanEnter(_level.Player, currentRoom))
            {
                connection.OnEnter();
                _level.CurrentRoomId = connection.TargetRoom.Id;
                
                // De Room zelf weet hoe groot hij is en waar de deur zit! (Geen Magic Numbers in de Controller)
                connection.TargetRoom.PlacePlayerAtEntrance(_level.Player, direction);
            }
        }
    }

    private bool CanMoveTo(int x, int y)
    {
        Room currentRoom = _level.CurrentRoom;

        if (currentRoom.IsWall(x, y))
        {
            if (currentRoom.IsEdgeDoor(x, y, out string dir))
            {
                if (currentRoom.OutgoingConnections.TryGetValue(dir, out var connection))
                {
                    return connection.CanEnter(_level.Player, currentRoom);
                }
            }
            return false;
        }

        if (currentRoom.SpecialTiles.TryGetValue((x, y), out string tileType) && tileType == "innerdoor")
        {
            var pressurePlates = currentRoom.Entities.OfType<PressurePlate>().ToList();
            if (pressurePlates.Any() && !pressurePlates.All(p => p.IsPressed))
            {
                return false; 
            }
        }

        return true;
    }
}