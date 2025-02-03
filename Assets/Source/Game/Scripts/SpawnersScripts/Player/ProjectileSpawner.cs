using Assets.Source.Game.Scripts;
using UnityEngine;

public class ProjectileSpawner
{
    private PlayerProjectile _bulletBrefab;
    private Pool _pool;
    private Transform _shotPoint;
    private float _damage;
    private DamageParametr _damageParametr;

    public ProjectileSpawner(PlayerProjectile playerProjectile, Pool pool, Transform shotPoint, float damage, DamageParametr damageParametr)
    {
        _bulletBrefab = playerProjectile;
        _pool = pool;
        _shotPoint = shotPoint;
        _damage = damage;
        _damageParametr = damageParametr;
    }

    public void SpawnProjectile(Enemy enemy)
    {
        PlayerProjectile playerProjectile;

        if (_pool.TryPoolObject(_bulletBrefab.gameObject, out PoolObject pollBullet))
        {
            playerProjectile = pollBullet as PlayerProjectile;
            playerProjectile.transform.position = _shotPoint.transform.position;
            playerProjectile.transform.rotation = _shotPoint.transform.rotation;
            playerProjectile.gameObject.SetActive(true);
        }
        else
        {
            playerProjectile = GameObject.Instantiate(_bulletBrefab, _shotPoint.transform.position, _shotPoint.transform.rotation);
            _pool.InstantiatePoolObject(playerProjectile, _bulletBrefab.name);
        }

        playerProjectile.Initialaze(enemy, _damage, 1f, _damageParametr);
    }
}