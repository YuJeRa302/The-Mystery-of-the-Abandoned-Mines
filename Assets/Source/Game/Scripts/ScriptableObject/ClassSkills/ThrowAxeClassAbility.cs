using Assets.Source.Game.Scripts.AbilityScripts;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability/Throw Axe", order = 51)]
    public class ThrowAxeClassAbility : ClassAbilityData
    {
        [SerializeField] private AxeMissile _axemMssile;

        public AxeMissile AxeMissile => _axemMssile;
    }
}