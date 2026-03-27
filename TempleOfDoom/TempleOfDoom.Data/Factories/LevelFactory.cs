using TempleOfDoom.Data.DTOs;
using TempleOfDoom.Domain.Models;

namespace TempleOfDoom.Data.Factories;

public class LevelFactory
{
    // Converts the DTO into actual Domain objects
    public Level CreateLevel(LevelDTO dto)
    {
        Level level = new Level();

        level.Player = new Player(dto.player.startX, dto.player.startY, dto.player.lives);
        level.CurrentRoomId = dto.player.startRoomId;

        foreach (var roomDto in dto.rooms)
        {
            Room room = new Room(roomDto.id, roomDto.width, roomDto.height);

            // Add inner walls and inner doors for Module C
            if (roomDto.specialFloorTiles != null)
            {
                foreach (var tile in roomDto.specialFloorTiles)
                {
                    room.SpecialTiles.Add((tile.x, tile.y), tile.type);
                }
            }

            // Parse enemies using the EnemyAdapter
            if (roomDto.enemies != null)
            {
                foreach (var enemyDto in roomDto.enemies)
                {
                    CODE_TempleOfDoom_DownloadableContent.Enemy? dllEnemy = null;
                    
                    if (enemyDto.type.ToLower() == "horizontal")
                        dllEnemy = new CODE_TempleOfDoom_DownloadableContent.HorizontallyMovingEnemy(3, enemyDto.x, enemyDto.y, enemyDto.minX, enemyDto.maxX);
                    else if (enemyDto.type.ToLower() == "vertical")
                        dllEnemy = new CODE_TempleOfDoom_DownloadableContent.VerticallyMovingEnemy(3, enemyDto.x, enemyDto.y, enemyDto.minY, enemyDto.maxY);

                    if (dllEnemy != null)
                        room.Entities.Add(new TempleOfDoom.Domain.Enemies.EnemyAdapter(dllEnemy, enemyDto.x, enemyDto.y));
                }
            }

            // Parse items (keys, sankara stones, boobytraps, pressure plates)
            if (roomDto.items != null)
            {
                foreach (var itemDto in roomDto.items)
                {
                    switch (itemDto.type.ToLower())
                    {
                        case "sankara stone":
                            room.Entities.Add(new TempleOfDoom.Domain.Items.SankaraStone(itemDto.x, itemDto.y));
                            break;
                        case "key":
                            room.Entities.Add(new TempleOfDoom.Domain.Items.Key(itemDto.x, itemDto.y, itemDto.color ?? ""));
                            break;
                        case "boobytrap":
                        case "disappearing boobytrap":
                            bool isDisappearing = itemDto.type.ToLower().Contains("disappearing");
                            room.Entities.Add(new TempleOfDoom.Domain.Items.Boobytrap(itemDto.x, itemDto.y, itemDto.damage ?? 1, isDisappearing));
                            break;
                        case "pressure plate":
                            room.Entities.Add(new TempleOfDoom.Domain.Items.PressurePlate(itemDto.x, itemDto.y));
                            break;
                    }
                }
            }

            level.Rooms.Add(room.Id, room);
        }

        foreach (var connDto in dto.connections)
        {
            List<TempleOfDoom.Domain.Doors.IDoor> parsedDoors = new();
            
            if (connDto.doors != null)
            {
                foreach (var doorDto in connDto.doors)
                {
                    switch (doorDto.type.ToLower())
                    {
                        case "colored": parsedDoors.Add(new TempleOfDoom.Domain.Doors.ColoredDoor(doorDto.color ?? "")); break;
                        case "toggle": parsedDoors.Add(new TempleOfDoom.Domain.Doors.ToggleDoor()); break;
                        case "closing gate": parsedDoors.Add(new TempleOfDoom.Domain.Doors.ClosingGate()); break;
                        case "switched": parsedDoors.Add(new TempleOfDoom.Domain.Doors.SwitchDoor()); break;
                    }
                }
            }

            if (connDto.NORTH.HasValue && connDto.SOUTH.HasValue)
            {
                var roomN = level.Rooms[connDto.NORTH.Value];
                var roomS = level.Rooms[connDto.SOUTH.Value];

                var connToS = new TempleOfDoom.Domain.Models.Connection(roomS);
                connToS.Doors.AddRange(parsedDoors);
                roomN.OutgoingConnections.Add("SOUTH", connToS);

                var connToN = new TempleOfDoom.Domain.Models.Connection(roomN);
                connToN.Doors.AddRange(parsedDoors);
                roomS.OutgoingConnections.Add("NORTH", connToN);
            }

            if (connDto.WEST.HasValue && connDto.EAST.HasValue)
            {
                var roomW = level.Rooms[connDto.WEST.Value];
                var roomE = level.Rooms[connDto.EAST.Value];

                var connToE = new TempleOfDoom.Domain.Models.Connection(roomE);
                connToE.Doors.AddRange(parsedDoors);
                roomW.OutgoingConnections.Add("EAST", connToE);

                var connToW = new TempleOfDoom.Domain.Models.Connection(roomW);
                connToW.Doors.AddRange(parsedDoors);
                roomE.OutgoingConnections.Add("WEST", connToW);
            }
        }
        
        return level;
    }
}