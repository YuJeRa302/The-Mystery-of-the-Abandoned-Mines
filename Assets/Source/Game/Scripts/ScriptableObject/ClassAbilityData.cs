using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New ClassAbility", menuName = "Create Class Ability", order = 51)]
    public class ClassAbilityData : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private string _name;
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _description;
        [SerializeField] private List<AbilityClassParameter> _abilityClassParameters;

        public int Id => _id;
        public List<AbilityClassParameter> AbilityClassParameters => _abilityClassParameters;
        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;
    }
}