using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Attack Ability", menuName = "Create Attack Ability", order = 51)]
    public class AttackAbilityData : AbilityAttributeData
    {
        [Space(20)]
        [SerializeField] private Spell _spell;
        [SerializeField] private int _spellRadius;
        [SerializeField] private TypeAttackAbility _typeAttackAbility;

        public Spell Spell => _spell;
        public int SpellRadius => _spellRadius;
        public TypeAttackAbility TypeAttackAbility => _typeAttackAbility;
    }
}