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

        public override void Initialize(Player player, int id, int lvlRoom, int damage, int health, float attackDelay, float attackDistance, float moveSpeed)
        {
            _bulletSpawner = new BulletSpawner(_bullet, _pool, _shootPoint, this);
            base.Initialize(player, id, lvlRoom, damage, health, attackDelay, attackDistance, moveSpeed);
        }
    }
}