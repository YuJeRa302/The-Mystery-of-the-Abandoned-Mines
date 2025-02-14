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
                player.PlayerHealth.TakeDamage(Convert.ToInt32(_damage));
                ReturObjectPool();
            }

            if (collision.collider.TryGetComponent(out Wall wall))
            {
                ReturObjectPool();
            }
        }
    }
}