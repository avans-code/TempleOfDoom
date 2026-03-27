namespace TempleOfDoom.Domain.Models;

public class Player
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Lives { get; set; }
    public int StonesCollected { get; set; } = 0;
    private List<string> _keys = new();

    public Player(int x, int y, int lives)
    {
        X = x;
        Y = y;
        Lives = lives;
    }

    public void AddKey(string color) => _keys.Add(color.ToLower());
    public bool HasKey(string color) => _keys.Contains(color.ToLower());
    public void TakeDamage(int amount) => Lives -= amount;
}