using Assets.Source.Game.Scripts.AbilityScripts;
using Assets.Source.Game.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Attack Ability", menuName = "Create Attribute/Attack Ability Attribute", order = 51)]
    public class AttackAbilityData : ActiveAbilityData
    {
        [SerializeField] private Spell _spell;
        [SerializeField] private float _radius;
        [SerializeReference] private IAttackAbilityStrategy _attackAbilityStrategy;
        [SerializeField] private LegendaryAbilityData _legendaryAbilityData;

        public Spell Spell => _spell;
        public float Radius => _radius;
        public LegendaryAbilityData LegendaryAbilityData => _legendaryAbilityData;
        public IAttackAbilityStrategy IAttackAbilityStrategy => _attackAbilityStrategy;
    }
}