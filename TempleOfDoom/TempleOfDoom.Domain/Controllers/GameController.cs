using System.Reflection;
using TempleOfDoom.Domain.Models;
using TempleOfDoom.Domain.Enemies;
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
                            UnlockSwitchDoorsRecursive(connection.Doors);
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
        string direction = "";

        switch (key)
        {
            case ConsoleKey.UpArrow: targetY--; direction = "NORTH"; break;
            case ConsoleKey.DownArrow: targetY++; direction = "SOUTH"; break;
            case ConsoleKey.LeftArrow: targetX--; direction = "WEST"; break;
            case ConsoleKey.RightArrow: targetX++; direction = "EAST"; break;
            case ConsoleKey.Spacebar:
                Shoot();
                MoveEnemies();
                UpdatePlates();
                return; 
        }
        
        Room currentRoom = _level.CurrentRoom;
        
        if (targetX < 0 || targetX >= currentRoom.Width || targetY < 0 || targetY >= currentRoom.Height)
        {
            TryTransitionRoom(direction, targetX, targetY);
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

    private void UnlockSwitchDoorsRecursive(IEnumerable<IDoor> doors)
    {
        foreach (var door in doors)
        {
            if (door is SwitchDoor sd)
            {
                sd.Unlock();
            }
            else
            {
                var innerDoorProp = door.GetType().GetProperty("InnerDoor") ?? 
                                    door.GetType().GetProperty("_innerDoor", BindingFlags.NonPublic | BindingFlags.Instance);
                
                if (innerDoorProp != null && innerDoorProp.GetValue(door) is IDoor innerDoor)
                {
                    UnlockSwitchDoorsRecursive(new[] { innerDoor });
                }
            }
        }
    }

    private void TryTransitionRoom(string direction, int targetX, int targetY)
    {
        Room currentRoom = _level.CurrentRoom;
        
        bool isAtDoor = false;
        if (direction == "NORTH" && targetX == currentRoom.Width / 2 && targetY < 0) isAtDoor = true;
        if (direction == "SOUTH" && targetX == currentRoom.Width / 2 && targetY >= currentRoom.Height) isAtDoor = true;
        if (direction == "WEST" && targetX < 0 && targetY == currentRoom.Height / 2) isAtDoor = true;
        if (direction == "EAST" && targetX >= currentRoom.Width && targetY == currentRoom.Height / 2) isAtDoor = true;

        if (!isAtDoor) return;

        if (currentRoom.OutgoingConnections.ContainsKey(direction))
        {
            var connection = currentRoom.OutgoingConnections[direction];
            
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
            bool isEdgeDoor = false;
            string dir = "";

            if (y == 0 && x == currentRoom.Width / 2) { isEdgeDoor = true; dir = "NORTH"; }
            else if (y == currentRoom.Height - 1 && x == currentRoom.Width / 2) { isEdgeDoor = true; dir = "SOUTH"; }
            else if (x == 0 && y == currentRoom.Height / 2) { isEdgeDoor = true; dir = "WEST"; }
            else if (x == currentRoom.Width - 1 && y == currentRoom.Height / 2) { isEdgeDoor = true; dir = "EAST"; }

            if (isEdgeDoor && currentRoom.OutgoingConnections.ContainsKey(dir))
            {
                var connection = currentRoom.OutgoingConnections[dir];
                if (connection.CanEnter(_level.Player, currentRoom))
                {
                    return true;
                }
            }
            return false;
        }

        if (currentRoom.SpecialTiles.ContainsKey((x, y)) && currentRoom.SpecialTiles[(x, y)] == "innerdoor")
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