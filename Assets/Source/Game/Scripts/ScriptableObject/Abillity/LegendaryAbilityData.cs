using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Legendary Ability", menuName = "Create Legendary Ability", order = 51)]
    public class LegendaryAbilityData : ActiveAbilityData
    {
        [SerializeField] private LegendaryAbilitySpell _legendarySpell;

        public LegendaryAbilitySpell LegendariSpell => _legendarySpell;
    }
}