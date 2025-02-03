using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Primary Attribute", menuName = "Create Attribute/Passive Ability Attribute", order = 51)]
    public class PassiveAttributeData : AttributeData
    {
        [SerializeField] private TypeMagic _typeMagic;
        [SerializeField] private TypeAbility _typeAbility;
        [SerializeField] private PassiveAbilityView _passiveAbilityView;

        public TypeMagic TypeMagic => _typeMagic;
        public TypeAbility TypeAbility => _typeAbility;
        public PassiveAbilityView PassiveAbilityView => _passiveAbilityView;
    }
}