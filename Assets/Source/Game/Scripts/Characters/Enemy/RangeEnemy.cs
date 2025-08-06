using Assets.Source.Game.Scripts.SpawnersScripts;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    public class RangeEnemy : Enemy
    {
        [SerializeField] private EnemyBullet _bullet;
        [SerializeField] private Transform _shootPoint;

        private BulletSpawner _bulletSpawner;

        public BulletSpawner BulletSpawner => _bulletSpawner;

        public override void Initialize(Player player, int lvlRoom, EnemyData data, int tire)
        {
            _bulletSpawner = new BulletSpawner(_bullet, EnemyPool, _shootPoint, this);
            base.Initialize(player, lvlRoom, data, tire);
        }
    }
}