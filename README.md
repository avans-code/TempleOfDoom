# TempleOfDoom

mermaid```
classDiagram
    %% Factory Pattern
    class LevelFactory {
        +CreateLevel(dto: LevelDTO): Level
    }
    
    %% Strategy Pattern
    class ILevelLoader {
        <<interface>>
        +Load(path: string): LevelDTO
    }
    class JsonLevelLoader {
        +Load(path: string): LevelDTO
    }
    ILevelLoader <|.. JsonLevelLoader
    
    %% Domain Models
    class Level {
        +Player: Player
        +CurrentRoomId: int
        +Rooms: Dictionary~int, Room~
        +StepCount: int
        +CurrentRoom: Room
    }
    
    class Room {
        +Id: int
        +Width: int
        +Height: int
        +Entities: List~Entity~
        +OutgoingConnections: Dictionary~string, Connection~
        +SpecialTiles: Dictionary~(int, int), string~
        +IsWall(x: int, y: int): bool
        +IsEdgeDoor(x: int, y: int, out direction: string): bool
        +GetExitDirection(targetX: int, targetY: int): string
        +HandleShooting(player: Player)
        +MoveEnemies()
        +ResolveCollisions(player: Player)
        +UpdatePressurePlates(player: Player)
        +HandleItemPickup(player: Player)
        +PlacePlayerAtEntrance(player: Player, entryDirection: string)
    }
    
    class Connection {
        +TargetRoom: Room
        +Doors: List~IDoor~
        +IsHorizontal: bool
        +CanEnter(player: Player, currentRoom: Room): bool
        +OnEnter()
    }
    
    class Entity {
        <<abstract>>
        +X: int
        +Y: int
    }
    
    class Player {
        +Lives: int
        +StonesCollected: int
        +Keys: List~string~
        +AddKey(color: string)
        +HasKey(color: string): bool
        +TakeDamage(amount: int)
    }
    
    %% Items & Observer Pattern
    class Item {
        <<abstract>>
        +Interact(player: Player, room: Room)*
    }
    
    class Boobytrap {
        +Damage: int
        +IsDisappearing: bool
        +HasTriggered: bool
        +Interact(player: Player, room: Room)
    }
    
    class Key {
        +Color: string
        +Interact(player: Player, room: Room)
    }
    
    class SankaraStone {
        +Interact(player: Player, room: Room)
    }
    
    class PressurePlate {
        -bool _isOccupied
        +IsOccupied: bool
        +IsPressed: bool
        +event Action OnOccupancyChanged
        +Interact(player: Player, room: Room)
    }
    
    %% Doors & Decorator Pattern
    class IDoor {
        <<interface>>
        +CanEnter(player: Player, currentRoom: Room): bool
        +OnEnter()
        +Unlock()
    }
    
    class ClosingGate {
        +IsOpen: bool
        +CanEnter(player: Player, currentRoom: Room): bool
        +OnEnter()
        +Unlock()
    }
    
    class ColoredDoor {
        +Color: string
        +IsOpen: bool
        +CanEnter(player: Player, currentRoom: Room): bool
        +OnEnter()
        +Unlock()
    }
    
    class SwitchDoor {
        +IsOpen: bool
        +CanEnter(player: Player, currentRoom: Room): bool
        +OnEnter()
        +Unlock()
    }
    
    class ToggleDoor {
        +IsOpen: bool
        +CanEnter(player: Player, currentRoom: Room): bool
        +OnEnter()
        +Unlock()
    }
    
    class OpenOnOddDoor {
        -_innerDoor: IDoor
        -_level: Level
        +CanEnter(player: Player, currentRoom: Room): bool
        +OnEnter()
        +Unlock()
    }
    
    %% Enemy Adapter Pattern
    class EnemyAdapter {
        -_dllEnemy: Enemy
        -_localLives: int
        +Lives: int
        +IsDead: bool
        +Move()
        +TakeDamage(damage: int)
    }
    
    class FieldAdapter {
        +CanEnter: bool
        +Item: IPlacable
        +GetNeighbour(direction: int): IField
    }
    
    %% Controller
    class GameController {
        -_level: Level
        +CurrentLevel: Level
        +IsGameOver: bool
        -WireObservers()
        +HandleInput(key: ConsoleKey)
        -MovePlayer(dx: int, dy: int)
        -ProcessRoomEvents()
        -TryTransitionRoom(direction: string)
        -CanMoveTo(x: int, y: int): bool
    }
    
    %% Relationships
    Level "1" *-- "many" Room : Rooms
    Level "1" *-- "1" Player : Player
    Room "1" *-- "many" Entity : Entities
    Room "1" *-- "many" Connection : OutgoingConnections
    Connection "1" --> "1" Room : TargetRoom
    Connection "1" *-- "many" IDoor : Doors
    
    Entity <|-- Player
    Entity <|-- Item
    Entity <|-- EnemyAdapter
    
    Item <|-- Boobytrap
    Item <|-- Key
    Item <|-- SankaraStone
    Item <|-- PressurePlate
    
    IDoor <|.. ClosingGate
    IDoor <|.. ColoredDoor
    IDoor <|.. SwitchDoor
    IDoor <|.. ToggleDoor
    IDoor <|.. OpenOnOddDoor
    
    %% Decorator Relation
    OpenOnOddDoor o-- IDoor : wraps
    
    GameController o-- Level
    LevelFactory ..> Level : creates
    
    %% Observer Dependency
    PressurePlate ..> GameController : invokes event
```
