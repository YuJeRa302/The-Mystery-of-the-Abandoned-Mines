using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.PoolSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    [System.Serializable]
    public class DamageSource
    {
        [SerializeField] private TypeDamage _typeDamage;
        [SerializeField] private float _damage;
        [SerializeField] private float _damageDelay;
        [SerializeField] private List<DamageParameter> _damageParameters;
        [SerializeField] private PoolParticle _particle;

        public DamageSource(
            TypeDamage typeDamage,
            List<DamageParameter> damageParameters,
            PoolParticle poolParticle,
            float damage)
        {
            _typeDamage = typeDamage;
            _damageParameters = damageParameters;
            _particle = poolParticle;
            _damage = damage;
        }

        public float DamageDelay => _damageDelay;
        public float Damage => _damage;
        public TypeDamage TypeDamage => _typeDamage;
        public List<DamageParameter> DamageParameters => _damageParameters;
        public PoolParticle PoolParticle => _particle;

        public void ChangeDamage(float value)
        {
            _damage = value;
        }
    }
}