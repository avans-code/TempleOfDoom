using TempleOfDoom.Domain.Models;

namespace TempleOfDoom.Domain.Items;

public class PressurePlate(int x, int y) : Item(x, y)
{
    private bool _isOccupied;
    public event Action? OnOccupancyChanged;

    public bool IsOccupied
    {
        get => _isOccupied;
        set
        {
            if (_isOccupied != value)
            {
                _isOccupied = value;
                OnOccupancyChanged?.Invoke();
            }
        }
    }

    public bool IsPressed { get; set; }

    public override void Interact(Player player, Room room)
    {
        IsPressed = true;
    }
}