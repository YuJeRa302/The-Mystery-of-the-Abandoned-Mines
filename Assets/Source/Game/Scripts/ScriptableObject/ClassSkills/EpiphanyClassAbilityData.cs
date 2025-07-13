using Assets.Source.Game.Scripts.AbilityScripts;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability/Epiphany", order = 51)]
    public class EpiphanyClassAbilityData : ClassAbilityData
    {
        [SerializeField] private ParticleSystem _epiphanyParticle;
        [SerializeField] private Spell _spell;

        public Spell Spell => _spell;
        public ParticleSystem EpiphanyParticle => _epiphanyParticle;
    }
}