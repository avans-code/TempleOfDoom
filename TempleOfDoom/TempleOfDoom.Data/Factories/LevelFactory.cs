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
                        room.Entities.Add(new Domain.Enemies.EnemyAdapter(dllEnemy, enemyDto.x, enemyDto.y));
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
                            room.Entities.Add(new Domain.Items.SankaraStone(itemDto.x, itemDto.y));
                            break;
                        case "key":
                            room.Entities.Add(new Domain.Items.Key(itemDto.x, itemDto.y, itemDto.color ?? ""));
                            break;
                        case "boobytrap":
                        case "disappearing boobytrap":
                            bool isDisappearing = itemDto.type.ToLower().Contains("disappearing");
                            room.Entities.Add(new Domain.Items.Boobytrap(itemDto.x, itemDto.y, itemDto.damage ?? 1, isDisappearing));
                            break;
                        case "pressure plate":
                            room.Entities.Add(new Domain.Items.PressurePlate(itemDto.x, itemDto.y));
                            break;
                    }
                }
            }

            level.Rooms.Add(room.Id, room);
        }

        // Process connections AFTER all rooms are created
        foreach (var connDto in dto.connections)
        {
            List<Domain.Doors.IDoor> parsedDoors = new();
            
            if (connDto.doors != null)
            {
                foreach (var doorDto in connDto.doors)
                {
                    switch (doorDto.type.ToLower())
                    {
                        case "colored": 
                            parsedDoors.Add(new Domain.Doors.ColoredDoor(doorDto.color ?? "")); 
                            break;
                        case "toggle": 
                            parsedDoors.Add(new Domain.Doors.ToggleDoor()); 
                            break;
                        case "closing gate": 
                            parsedDoors.Add(new Domain.Doors.ClosingGate()); 
                            break;
                        case "switched": 
                            parsedDoors.Add(new Domain.Doors.SwitchDoor()); 
                            break;
                        case "open on odd": 
                            parsedDoors.Add(new Domain.Doors.OpenOnOddDoor(level)); 
                            break;
                    }
                }
            }

            // Handle regular room-to-room connections
            if (connDto.NORTH.HasValue && connDto.SOUTH.HasValue)
            {
                var roomN = level.Rooms[connDto.NORTH.Value];
                var roomS = level.Rooms[connDto.SOUTH.Value];

                var connToS = new Connection(roomS);
                connToS.Doors.AddRange(parsedDoors);
                roomN.OutgoingConnections.Add("SOUTH", connToS);

                var connToN = new Connection(roomN);
                connToN.Doors.AddRange(parsedDoors);
                roomS.OutgoingConnections.Add("NORTH", connToN);
            }

            if (connDto.WEST.HasValue && connDto.EAST.HasValue)
            {
                var roomW = level.Rooms[connDto.WEST.Value];
                var roomE = level.Rooms[connDto.EAST.Value];

                var connToE = new Connection(roomE);
                connToE.Doors.AddRange(parsedDoors);
                roomW.OutgoingConnections.Add("EAST", connToE);

                var connToW = new Connection(roomW);
                connToW.Doors.AddRange(parsedDoors);
                roomE.OutgoingConnections.Add("WEST", connToW);
            }

            // Handle "within" room connections (inner doors like in room 4)
            if (connDto.within.HasValue)
            {
                var room = level.Rooms[connDto.within.Value];
                
                // For inner doors, we need to create a special connection
                // This represents the inner door in room 4 (position x=9, y=2)
                // The doors array contains the requirements to open this inner door
                var innerConnection = new Connection(room); // Points to same room
                innerConnection.Doors.AddRange(parsedDoors);
                
                // Store this as a special connection that can be checked when moving through inner doors
                room.OutgoingConnections.Add("INNER", innerConnection);
            }
        }
        
        return level;
    }
}