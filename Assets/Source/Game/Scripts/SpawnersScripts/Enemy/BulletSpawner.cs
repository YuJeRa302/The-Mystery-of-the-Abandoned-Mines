using System;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    private EnemyBullet _bulletBrefab;
    private Pool _pool;
    private Transform _shootPoint;
    private float _damage;

    public void Initialize(EnemyBullet enemyBullet, Pool pool, Transform shootPoint, float damage)
    {
        _bulletBrefab = enemyBullet;
        _pool = pool;
        _shootPoint = shootPoint;
        _damage = damage;
    }

    public void SpawnBullet()
    {
        EnemyBullet bullet;

        if (_pool.TryPoolObject(out PoolObject pollBullet))
        {
            bullet = pollBullet as EnemyBullet;
            bullet.transform.position = _shootPoint.transform.position;
            bullet.transform.rotation = _shootPoint.transform.rotation;
            bullet.gameObject.SetActive(true);
        }
        else
        {
            bullet = Instantiate(_bulletBrefab, _shootPoint.transform.position, _shootPoint.transform.rotation);
            _pool.InstantiatePoolObject(bullet);
        }

        bullet.Initialaze((int)Math.Round(_damage));
        bullet.GetComponent<Rigidbody>().AddForce(_shootPoint.forward * 5f, ForceMode.Impulse);
    }
}