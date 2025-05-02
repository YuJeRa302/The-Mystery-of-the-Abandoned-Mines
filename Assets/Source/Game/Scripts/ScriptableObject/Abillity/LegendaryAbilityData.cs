using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Legendary Ability", menuName = "Create Legendary Ability", order = 51)]
    public class LegendaryAbilityData : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private string _discription;
        [SerializeField] private LegendaryAbilitySpell _legendarySpell;
        [SerializeField] private float _spellRadius;
        [SerializeField] private Sprite _icon;
        [SerializeField] private List<Parameters> _legendaryParametersAbility;
        [SerializeField] private ParticleSystem _particle;
        [SerializeField] private TypeUpgradeAbility _typeUpgradeMagic;
        [SerializeField] private TypeMagic _typeMagic;
        [SerializeField] private CardParameterView _parameterView;
        [SerializeField] private DamageSource _damageParametr;

        public string Name => _name;
        public string Description => _discription;
        public LegendaryAbilitySpell LegendaryAbilitySpell => _legendarySpell;
        public float SpellRadius => _spellRadius;
        public Sprite Icon => _icon;
        public List<Parameters> LegendaryAbilityParameters => _legendaryParametersAbility;
        public ParticleSystem Particle => _particle;
        public TypeUpgradeAbility TypeUpgradeMagic => _typeUpgradeMagic;
        public TypeMagic TypeMagic => _typeMagic;
        public CardParameterView CardParameterView => _parameterView;
        public DamageSource DamageParametr => _damageParametr;
    }
}