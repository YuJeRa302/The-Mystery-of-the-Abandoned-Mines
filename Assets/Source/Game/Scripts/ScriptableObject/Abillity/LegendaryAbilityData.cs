using Assets.Source.Game.Scripts.AbilityScripts;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Legendary Ability", menuName = "Create Legendary Ability", order = 51)]
    public class LegendaryAbilityData : ActiveAbilityData
    {
        [SerializeField] private LegendaryThunderAbilitySpell _legendarySpell;

        public LegendaryThunderAbilitySpell LegendarySpell => _legendarySpell;
    }
}