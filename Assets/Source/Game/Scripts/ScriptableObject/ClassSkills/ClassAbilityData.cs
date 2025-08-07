using Assets.Source.Game.Scripts.AbilityScripts;
using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.Upgrades;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability/New Class Ability", order = 51)]
    public class ClassAbilityData : AttributeData
    {
        [SerializeField] private int _id;
        [SerializeField] private ClassSkillButtonView _buttonView;
        [SerializeField] private List<UpgradeParameter> _abilityClassParameters;
        [SerializeField] private TypeAbility _abilityType;
        [SerializeField] private DamageSource _damageParameter;
        [SerializeReference] private IAbilityStrategy _abilityStrategy;

        public int Id => _id;
        public List<UpgradeParameter> AbilityClassParameters => _abilityClassParameters;
        public ClassSkillButtonView ButtonView => _buttonView;
        public TypeAbility AbilityType => _abilityType;
        public IAbilityStrategy IAbilityStrategy => _abilityStrategy;
        public DamageSource DamageSource => _damageParameter;
    }
}