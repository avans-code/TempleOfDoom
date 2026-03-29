using System;
using TempleOfDoom.Domain.Models;

namespace TempleOfDoom.Domain.Items;

public class PressurePlate : Item
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

    public bool IsPressed { get; set; } = false;

    public PressurePlate(int x, int y) : base(x, y)
    {
    }
}