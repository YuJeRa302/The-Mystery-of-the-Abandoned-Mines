using UnityEngine;

public class RangeEnemy : Enemy
{
    [SerializeField] private EnemyBullet _bullet;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private Pool _poolBullet;
    [SerializeField] private BulletSpawner _bulletSpawner;

    public BulletSpawner BulletSpawner => _bulletSpawner;

    public EnemyBullet Bullet => _bullet;
    public Transform ShootPoint => _shootPoint;
    public Pool Pool => _poolBullet;

    private void OnEnable()
    {
        _bulletSpawner.Initialize(_bullet, _poolBullet, _shootPoint, _damage);
    }
}