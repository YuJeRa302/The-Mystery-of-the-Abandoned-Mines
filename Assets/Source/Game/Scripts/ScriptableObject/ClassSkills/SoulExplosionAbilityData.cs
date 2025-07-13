using Assets.Source.Game.Scripts.AbilityScripts;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability/SoulExplosion", order = 51)]
    public class SoulExplosionAbilityData : ClassAbilityData
    {
        [SerializeField] private Spell _spell;
        [SerializeField] private ParticleSystem _damageParticle;

        public ParticleSystem DamageParticle => _damageParticle;
        public Spell Spell => _spell;
    }
}