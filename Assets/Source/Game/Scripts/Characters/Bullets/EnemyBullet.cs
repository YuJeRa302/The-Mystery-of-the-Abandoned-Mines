using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    public class EnemyBullet : Bullet
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent(out Player player))
            {
                player.TakeDamage(Convert.ToInt32(Damage));
                ReturnObjectPool();
            }

            if (collision.collider.TryGetComponent(out Wall wall))
            {
                ReturnObjectPool();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                player.TakeDamage(Convert.ToInt32(Damage));
                ReturnObjectPool();
            }
        }
    }
}