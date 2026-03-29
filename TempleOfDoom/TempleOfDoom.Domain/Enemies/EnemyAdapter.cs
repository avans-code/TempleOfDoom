using TempleOfDoom.Domain.Models;
using CODE_TempleOfDoom_DownloadableContent;

namespace TempleOfDoom.Domain.Enemies;

public class EnemyAdapter : Entity
{
    private readonly Enemy _dllEnemy;
    private int _localLives;

    public EnemyAdapter(Enemy dllEnemy, int x, int y) : base(x, y)
    {
        _dllEnemy = dllEnemy;
        _localLives = _dllEnemy.NumberOfLives; 

        _dllEnemy.CurrentField = new FieldAdapter(); 
        _dllEnemy.CurrentField.Item = _dllEnemy;
    }

    public void Move()
    {
            _dllEnemy.Move();
            X = _dllEnemy.CurrentXLocation;
            Y = _dllEnemy.CurrentYLocation;
    }

    public void TakeDamage(int damage)
    {
        _localLives -= damage; 
    }

    // public int Lives => _localLives;
    public bool IsDead => _localLives <= 0;
}