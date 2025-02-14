using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Attack Ability", menuName = "Create Attribute/Attack Ability Attribute", order = 51)]
    public class AttackAbilityData : AbilityAttributeData
    {
        [Space(20)]
        [SerializeField] private Spell _spell;
        [SerializeField] private float _spellRadius;
        [SerializeField] private TypeAttackAbility _typeAttackAbility;
        [SerializeField] private DamageParametr _damageParametr;

        public Spell Spell => _spell;
        public float SpellRadius => _spellRadius;
        public TypeAttackAbility TypeAttackAbility => _typeAttackAbility;
        public DamageParametr DamageParametr => _damageParametr;
    }
}