using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class BulletSpawner
    {
        private Bullet _bulletBrefab;
        private Pool _pool;
        private Transform _shotPoint;
        private float _damage;

        public BulletSpawner(Bullet enemyBullet, Pool pool, Transform shotPoint, float damage)
        {
            _bulletBrefab = enemyBullet;
            _pool = pool;
            _shotPoint = shotPoint;
            _damage = damage;
        }

        public void SpawnBullet()
        {
            Bullet bullet;

            if (_pool.TryPoolObject(_bulletBrefab.gameObject, out PoolObject pollBullet))
            {
                bullet = pollBullet as Bullet;
                bullet.transform.position = _shotPoint.transform.position;
                bullet.transform.rotation = _shotPoint.transform.rotation;
                bullet.gameObject.SetActive(true);
            }
            else
            {
                bullet = GameObject.Instantiate(_bulletBrefab, _shotPoint.transform.position, _shotPoint.transform.rotation);
                _pool.InstantiatePoolObject(bullet, _bulletBrefab.name);
            }

            bullet.Initialaze((int)Math.Round(_damage));
            bullet.GetComponent<Rigidbody>().AddForce(_shotPoint.forward * 5f, ForceMode.Impulse);
        }

        public void ChengeShotPoint(Transform shotPoint)
        {
            _shotPoint = shotPoint;
        }
    }
}