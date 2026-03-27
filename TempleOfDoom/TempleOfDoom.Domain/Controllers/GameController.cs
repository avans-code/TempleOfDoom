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
    }

    public Level CurrentLevel => _level;
    public bool IsGameOver => _level.Player.Lives <= 0 || _level.Player.StonesCollected >= 5;

    public void HandleInput(ConsoleKey key)
    {
        _level.StepCount++;
        int targetX = _level.Player.X;
        int targetY = _level.Player.Y;
        string direction = "";

        // Bepaal de nieuwe doelpositie en richting
        switch (key)
        {
            case ConsoleKey.UpArrow: targetY--; direction = "NORTH"; break;
            case ConsoleKey.DownArrow: targetY++; direction = "SOUTH"; break;
            case ConsoleKey.LeftArrow: targetX--; direction = "WEST"; break;
            case ConsoleKey.RightArrow: targetX++; direction = "EAST"; break;
            case ConsoleKey.Spacebar:
                Shoot();
                MoveEnemies();
                return; 
        }
        
        Room currentRoom = _level.CurrentRoom;
        if (targetX <= 0 || targetX >= currentRoom.Width - 1 || targetY <= 0 || targetY >= currentRoom.Height - 1)
        {
            TryTransitionRoom(direction, targetX, targetY);
            MoveEnemies();
            return;
        }

        // Check of we mogen lopen (binnen de kamer)
        if (CanMoveTo(targetX, targetY))
        {
            _level.Player.X = targetX;
            _level.Player.Y = targetY;
            HandleItemPickup();
            
            // Check collision after player moves into an enemy
            if (_level.CurrentRoom.Entities.OfType<EnemyAdapter>().Any(e => e.X == _level.Player.X && e.Y == _level.Player.Y))
            {
                _level.Player.TakeDamage(1);
            }
        }

        // Vijanden bewegen ALTIJD na een speler-actie
        MoveEnemies();
    }

    private void TryTransitionRoom(string direction, int targetX, int targetY)
    {
        Room currentRoom = _level.CurrentRoom;
        
        // Check if player is exactly at the door coordinates
        bool isAtDoor = false;
        if (direction == "NORTH" && targetX == currentRoom.Width / 2 && targetY == 0) isAtDoor = true;
        if (direction == "SOUTH" && targetX == currentRoom.Width / 2 && targetY == currentRoom.Height - 1) isAtDoor = true;
        if (direction == "WEST" && targetX == 0 && targetY == currentRoom.Height / 2) isAtDoor = true;
        if (direction == "EAST" && targetX == currentRoom.Width - 1 && targetY == currentRoom.Height / 2) isAtDoor = true;

        if (!isAtDoor) return; // Player hit a normal wall, not the door

        if (currentRoom.OutgoingConnections.ContainsKey(direction))
        {
            var connection = currentRoom.OutgoingConnections[direction];
            
            if (connection.CanEnter(_level.Player, currentRoom))
            {
                connection.OnEnter();
                _level.CurrentRoomId = connection.TargetRoom.Id;
                
                // Position player at the opposite door in the new room
                if (direction == "NORTH") { _level.Player.Y = connection.TargetRoom.Height - 2; _level.Player.X = connection.TargetRoom.Width / 2; }
                if (direction == "SOUTH") { _level.Player.Y = 1; _level.Player.X = connection.TargetRoom.Width / 2; }
                if (direction == "WEST") { _level.Player.X = connection.TargetRoom.Width - 2; _level.Player.Y = connection.TargetRoom.Height / 2; }
                if (direction == "EAST") { _level.Player.X = 1; _level.Player.Y = connection.TargetRoom.Height / 2; }
            }
        }
    }

    private bool CanMoveTo(int x, int y)
    {
        Room currentRoom = _level.CurrentRoom;

        // 1. Check Buitenmuren en Binnenmuren ("wall")
        if (currentRoom.IsWall(x, y)) return false;

        // 2. Check Binnendeuren ("innerdoor") uit Module C
        if (currentRoom.SpecialTiles.ContainsKey((x, y)) && currentRoom.SpecialTiles[(x, y)] == "innerdoor")
        {
            // Zoals in de PDF staat, gaat een innerdoor (SwitchDoor) open als de pressure plates in DEZELFDE ruimte ingedrukt zijn
            var pressurePlates = currentRoom.Entities.OfType<PressurePlate>().ToList();
            
            // Als er pressure plates zijn, moeten ze ALLEMAAL ingedrukt zijn. 
            // Zijn er geen, dan is de deur standaard open (of gedraagt zich als normale vloer)
            if (pressurePlates.Any() && !pressurePlates.All(p => p.IsPressed))
            {
                return false; // Deur is dicht, je mag er niet doorheen
            }
        }

        return true;
    }

    private void HandleItemPickup()
    {
        Room currentRoom = _level.CurrentRoom;
        var itemOnFloor = currentRoom.Entities.OfType<Item>()
            .FirstOrDefault(i => i.X == _level.Player.X && i.Y == _level.Player.Y);

        if (itemOnFloor != null)
        {
            if (itemOnFloor is SankaraStone)
            {
                _level.Player.StonesCollected++;
                currentRoom.Entities.Remove(itemOnFloor);
            }
            else if (itemOnFloor is Key key)
            {
                _level.Player.AddKey(key.Color);
                currentRoom.Entities.Remove(itemOnFloor);
            }
            else if (itemOnFloor is Boobytrap trap && !trap.HasTriggered)
            {
                _level.Player.TakeDamage(trap.Damage);
                trap.HasTriggered = true;
                if (trap.IsDisappearing)
                {
                    currentRoom.Entities.Remove(itemOnFloor);
                }
            }
            else if (itemOnFloor is PressurePlate plate)
            {
                plate.IsPressed = true;
            }
        }
    }

    private void MoveEnemies()
    {
        Room currentRoom = _level.CurrentRoom;
        var enemies = currentRoom.Entities.OfType<EnemyAdapter>().ToList();

        foreach (var enemy in enemies)
        {
            enemy.Move();
            
            // Check collision after enemy moves
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
            // Check if enemy is exactly 1 tile away horizontally or vertically
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