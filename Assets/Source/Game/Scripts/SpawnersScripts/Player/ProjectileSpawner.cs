using Assets.Source.Game.Scripts;
using UnityEngine;

public class ProjectileSpawner
{
    private PlayerProjectile _bulletBrefab;
    private Pool _pool;
    private Transform _shotPoint;
    private DamageSource _damageSource;

    public ProjectileSpawner(PlayerProjectile playerProjectile, Pool pool, Transform shotPoint, DamageSource damageSource)
    {
        _bulletBrefab = playerProjectile;
        _pool = pool;
        _shotPoint = shotPoint;
        _damageSource = damageSource;
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

        playerProjectile.Initialaze(enemy, _damageSource);
    }
}