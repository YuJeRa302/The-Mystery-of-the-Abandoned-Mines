using Assets.Source.Game.Scripts.PoolSystem;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability/Rage", order = 51)]
    public class RageClassAbilityData : ClassAbilityData
    {
        [SerializeField] private PoolParticle _rageEffect;

        public PoolParticle RageEffect => _rageEffect;
    }
}