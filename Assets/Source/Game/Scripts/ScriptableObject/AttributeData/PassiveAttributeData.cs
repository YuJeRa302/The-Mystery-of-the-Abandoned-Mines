using Assets.Source.Game.Scripts.Views;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Primary Attribute", menuName = "Create Attribute/Passive Ability Attribute", order = 51)]
    public class PassiveAttributeData : AttributeData
    {
        [SerializeField] private PassiveAbilityView _abilityView;

        public PassiveAbilityView AbilityView => _abilityView;
    }
}