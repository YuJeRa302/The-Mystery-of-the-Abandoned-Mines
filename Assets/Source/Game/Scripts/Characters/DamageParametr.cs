using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageParametr
{
    [SerializeField] private TypeDamage _typeDamage;
    [SerializeField] private List<DamageSupportivePatametr> _damageSupportivePatametrs;
    [SerializeField] private PoolParticle _particle;

    public DamageParametr(TypeDamage typeDamage, List<DamageSupportivePatametr> damageSupportivePatametrs, PoolParticle poolParticle)
    {
        _typeDamage = typeDamage;
        _damageSupportivePatametrs = damageSupportivePatametrs;
        _particle = poolParticle;
    }

    public TypeDamage TypeDamage => _typeDamage;
    public List<DamageSupportivePatametr> DamageSupportivePatametrs => _damageSupportivePatametrs;
    public PoolParticle Particle => _particle;
}