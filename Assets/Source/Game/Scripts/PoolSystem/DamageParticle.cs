using Assets.Source.Game.Scripts.Characters;
using UnityEngine;

namespace Assets.Source.Game.Scripts.PoolSystem
{
    public class DamageParticle : PoolParticle
    {
        private DamageSource _damageParametr;

        public void Initialize(DamageSource damageParametr)
        {
            _damageParametr = damageParametr;
        }

        private void OnParticleCollision(GameObject other)
        {
            if (other.gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(_damageParametr);
            }
        }
    }
}