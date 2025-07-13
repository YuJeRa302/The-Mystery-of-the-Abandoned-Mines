using Assets.Source.Game.Scripts.PoolSystem;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability/ShieldUp", order = 51)]
    public class ShieldUpAbility : ClassAbilityData
    {
        [SerializeField] private PoolParticle _poolParticle;

        public PoolParticle PoolParticle => _poolParticle;
    }
}