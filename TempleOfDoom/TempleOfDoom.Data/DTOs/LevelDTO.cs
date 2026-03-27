namespace TempleOfDoom.Data.DTOs;

public class LevelDTO
{
    public List<RoomDTO> rooms { get; set; } = new();
    public List<ConnectionDTO> connections { get; set; } = new();
    public PlayerDTO player { get; set; } = new();
}

public class RoomDTO
{
    public int id { get; set; }
    public string type { get; set; } = "";
    public int width { get; set; }
    public int height { get; set; }
    public List<ItemDTO>? items { get; set; }
    public List<EnemyDTO>? enemies { get; set; }
    public List<SpecialFloorTileDTO>? specialFloorTiles { get; set; }
}

public class ItemDTO
{
    public string type { get; set; } = "";
    public int x { get; set; }
    public int y { get; set; }
    public int? damage { get; set; }
    public string? color { get; set; }
}

public class EnemyDTO
{
    public string type { get; set; } = "";
    public int x { get; set; }
    public int y { get; set; }
    public int minX { get; set; }
    public int maxX { get; set; }
    public int minY { get; set; }
    public int maxY { get; set; }
}

public class SpecialFloorTileDTO
{
    public string type { get; set; } = "";
    public int x { get; set; }
    public int y { get; set; }
}

public class ConnectionDTO
{
    public int? NORTH { get; set; }
    public int? EAST { get; set; }
    public int? SOUTH { get; set; }
    public int? WEST { get; set; }
    public int? within { get; set; }
    public bool horizontal { get; set; }
    public List<DoorDTO> doors { get; set; } = new();
}

public class DoorDTO
{
    public string type { get; set; } = "";
    public string? color { get; set; }
}

public class PlayerDTO
{
    public int startRoomId { get; set; }
    public int startX { get; set; }
    public int startY { get; set; }
    public int lives { get; set; }
}