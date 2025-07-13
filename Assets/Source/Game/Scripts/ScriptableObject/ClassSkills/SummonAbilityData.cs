using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability/SummonSkeleton", order = 51)]
    public class SummonAbilityData : ClassAbilityData
    {
        [SerializeField] private SummonData _summonData;

        public SummonData Summon => _summonData;
    }
}