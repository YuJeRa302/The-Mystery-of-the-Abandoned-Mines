using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class RangeEnemy : Enemy
    {
        [SerializeField] private EnemyBullet _bullet;
        [SerializeField] private Transform _shootPoint;
        
        private BulletSpawner _bulletSpawner;

        public BulletSpawner BulletSpawner => _bulletSpawner;

        public EnemyBullet Bullet => _bullet;
        public Transform ShootPoint => _shootPoint;

        private void OnEnable()
        {
            _bulletSpawner = new BulletSpawner(_bullet, _pool, _shootPoint, _damage);
        }
    }
}