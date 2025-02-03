using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Legendary Ability", menuName = "Create Legendary Ability  ", order = 51)]
    public class LegendaryAbilityData : ScriptableObject
    {
        [SerializeField] private LegendaryAbilitySpell _legendarySpell;
        [SerializeField] private int _spellRadius;
        [SerializeField] private Sprite _icon;
        [SerializeField] private List<Parameters> _legendaryParametersAbility;

        public LegendaryAbilitySpell LegendaryAbilitySpell => _legendarySpell;
        public int SpellRadius => _spellRadius;
        public Sprite Icon => _icon;
        public List<Parameters> LegendaryAbilityParameters => _legendaryParametersAbility;
    }
}