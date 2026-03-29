using TempleOfDoom.Domain.Models;
using TempleOfDoom.Domain.Enemies;
using TempleOfDoom.Domain.Items;

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
                        // Geen Inappropriate Intimacy (Reflection) meer!
                        // Door Polymorfisme luistert alleen de SwitchDoor hiernaar.
                        foreach (var connection in room.OutgoingConnections.Values)
                        {
                            foreach (var door in connection.Doors)
                            {
                                door.Unlock(); 
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
        int targetX = _level.Player.X;
        int targetY = _level.Player.Y;

        switch (key)
        {
            case ConsoleKey.UpArrow: targetY--; break;
            case ConsoleKey.DownArrow: targetY++; break;
            case ConsoleKey.LeftArrow: targetX--; break;
            case ConsoleKey.RightArrow: targetX++; break;
            case ConsoleKey.Spacebar:
                Shoot();
                MoveEnemies();
                UpdatePlates();
                return; 
        }
        
        Room currentRoom = _level.CurrentRoom;
        
        if (targetX < 0 || targetX >= currentRoom.Width || targetY < 0 || targetY >= currentRoom.Height)
        {
            TryTransitionRoom(targetX, targetY);
            MoveEnemies();
            UpdatePlates();
            return;
        }

        if (CanMoveTo(targetX, targetY))
        {
            _level.Player.X = targetX;
            _level.Player.Y = targetY;
            HandleItemPickup();
            
            if (_level.CurrentRoom.Entities.OfType<EnemyAdapter>().Any(e => e.X == _level.Player.X && e.Y == _level.Player.Y))
            {
                _level.Player.TakeDamage(1);
            }
        }

        MoveEnemies();
        UpdatePlates();
    }

    private void UpdatePlates()
    {
        Room currentRoom = _level.CurrentRoom;
        
        foreach (var plate in currentRoom.Entities.OfType<PressurePlate>())
        {
            plate.IsOccupied = (_level.Player.X == plate.X && _level.Player.Y == plate.Y) || 
                               currentRoom.Entities.OfType<EnemyAdapter>().Any(e => e.X == plate.X && e.Y == plate.Y);
        }
    }

    private void TryTransitionRoom(int targetX, int targetY)
    {
        Room currentRoom = _level.CurrentRoom;
        
        // Geen Complex Conditionals meer!
        string direction = currentRoom.GetExitDirection(targetX, targetY);
        
        if (string.IsNullOrEmpty(direction)) return;

        if (currentRoom.OutgoingConnections.TryGetValue(direction, out var connection))
        {
            if (connection.CanEnter(_level.Player, currentRoom))
            {
                connection.OnEnter();
                _level.CurrentRoomId = connection.TargetRoom.Id;
                
                if (direction == "NORTH") { _level.Player.Y = connection.TargetRoom.Height - 1; _level.Player.X = connection.TargetRoom.Width / 2; }
                if (direction == "SOUTH") { _level.Player.Y = 0; _level.Player.X = connection.TargetRoom.Width / 2; }
                if (direction == "WEST") { _level.Player.X = connection.TargetRoom.Width - 1; _level.Player.Y = connection.TargetRoom.Height / 2; }
                if (direction == "EAST") { _level.Player.X = 0; _level.Player.Y = connection.TargetRoom.Height / 2; }
            }
        }
    }

    private bool CanMoveTo(int x, int y)
    {
        Room currentRoom = _level.CurrentRoom;

        if (currentRoom.IsWall(x, y))
        {
            // Geen Complex Conditionals en Deeply Nested Code meer!
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

    private void HandleItemPickup()
    {
        Room currentRoom = _level.CurrentRoom;
        var itemOnFloor = currentRoom.Entities.OfType<Item>()
            .FirstOrDefault(i => i.X == _level.Player.X && i.Y == _level.Player.Y);
        
        itemOnFloor?.Interact(_level.Player, currentRoom);
    }

    private void MoveEnemies()
    {
        Room currentRoom = _level.CurrentRoom;
        var enemies = currentRoom.Entities.OfType<EnemyAdapter>().ToList();

        foreach (var enemy in enemies)
        {
            enemy.Move();
            
            if (enemy.X == _level.Player.X && enemy.Y == _level.Player.Y)
            {
                _level.Player.TakeDamage(1);
            }
        }
    }

    private void Shoot()
    {
        Room currentRoom = _level.CurrentRoom;
        var enemies = currentRoom.Entities.OfType<EnemyAdapter>().ToList();
        
        foreach (var enemy in enemies)
        {
            int dx = Math.Abs(enemy.X - _level.Player.X);
            int dy = Math.Abs(enemy.Y - _level.Player.Y);
            
            if ((dx == 1 && dy == 0) || (dx == 0 && dy == 1))
            {
                enemy.TakeDamage(1);
                if (enemy.IsDead)
                {
                    currentRoom.Entities.Remove(enemy);
                }
            }
        }
    }
}