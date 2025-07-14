using Assets.Source.Game.Scripts.Characters;
using UnityEngine;

namespace Assets.Source.Game.Scripts.PoolSystem
{
    public class DamageParticle : PoolParticle
    {
        private DamageSource _damageParameter;

        public void Initialize(DamageSource damageParameter)
        {
            _damageParameter = damageParameter;
        }

        private void OnParticleCollision(GameObject other)
        {
            if (other.gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(_damageParameter);
            }
        }
    }
}