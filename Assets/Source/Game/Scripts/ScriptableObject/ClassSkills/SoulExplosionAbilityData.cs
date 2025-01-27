using Assets.Source.Game.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability/SoulExplosion", order = 51)]
public class SoulExplosionAbilityData : ClassAbilityData
{
    [SerializeField] private DamageParticle _damageParticle;

    public DamageParticle DamageParticle => _damageParticle;
}