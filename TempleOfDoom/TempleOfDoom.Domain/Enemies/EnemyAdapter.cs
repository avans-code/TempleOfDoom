using TempleOfDoom.Domain.Models;
using CODE_TempleOfDoom_DownloadableContent; 

namespace TempleOfDoom.Domain.Enemies;

public class EnemyAdapter : Entity
{
    private readonly Enemy _dllEnemy;

    public EnemyAdapter(Enemy dllEnemy, int x, int y) : base(x, y)
    {
        _dllEnemy = dllEnemy;
        _dllEnemy.CurrentField = new FieldAdapter(); //
    }

    public void Move()
    {
        _dllEnemy.Move();
        X = _dllEnemy.CurrentXLocation;
        Y = _dllEnemy.CurrentYLocation;
    }

    public void TakeDamage(int damage)
    {
        _dllEnemy.DoDamage(damage);
    }

    public int Lives => _dllEnemy.NumberOfLives;
    public bool IsDead => Lives <= 0;
}