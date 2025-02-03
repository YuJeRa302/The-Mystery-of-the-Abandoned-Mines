using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability/New Class Ability", order = 51)]
    public class ClassAbilityData : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private string _name;
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _description;
        [SerializeField] private ClassSkillButtonView _buttonView;
        [SerializeField] private List<UpgradeParameter> _abilityClassParameters;
        [SerializeField] private List<Parameters> _parameters;
        [SerializeField] private TypeAbility _abilityType;
        [SerializeField] private DamageParametr _damageParametr; //сюда лист если что

        public int Id => _id;
        public List<UpgradeParameter> AbilityClassParameters => _abilityClassParameters;
        public List<Parameters> Parameters => _parameters;
        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;
        public ClassSkillButtonView ButtonView => _buttonView;
        public TypeAbility AbilityType => _abilityType;
        public DamageParametr DamageParametr => _damageParametr;
    }
}