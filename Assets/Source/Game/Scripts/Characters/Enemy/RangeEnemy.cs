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

        public override void Initialize(Player player, int lvlRoom, EnemyData data, int tire)
        {
            _bulletSpawner = new BulletSpawner(_bullet, _pool, _shootPoint, this);
            base.Initialize(player, lvlRoom, data, tire);
        }
    }
}