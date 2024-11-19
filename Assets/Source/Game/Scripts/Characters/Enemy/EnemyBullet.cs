using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class EnemyBullet : PoolObject
    {
        private float _damage;

        public void Initialaze(int damage)
        {
            _damage = damage;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent(out Player player))
            {
                //player.TakeDamage(_damage);
                ReturObjectPool();
            }

            //if (collision.collider.TryGetComponent(out Wall wall))
            //    ReturObjectPool();
        }
    }
}