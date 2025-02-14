using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Legendary Ability", menuName = "Create Legendary Ability", order = 51)]
    public class LegendaryAbilityData : ScriptableObject
    {
        [SerializeField] private LegendaryAbilitySpell _legendarySpell;
        [SerializeField] private float _spellRadius;
        [SerializeField] private Sprite _icon;
        [SerializeField] private List<Parameters> _legendaryParametersAbility;
        [SerializeField] private ParticleSystem _particle;

        public LegendaryAbilitySpell LegendaryAbilitySpell => _legendarySpell;
        public float SpellRadius => _spellRadius;
        public Sprite Icon => _icon;
        public List<Parameters> LegendaryAbilityParameters => _legendaryParametersAbility;
        public ParticleSystem Particle => _particle;
    }
}