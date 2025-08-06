using Assets.Source.Game.Scripts.Card;
using Assets.Source.Game.Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Attribute", menuName = "Create Attribute/Attribute  ", order = 51)]
    public abstract class AttributeData : ScriptableObject
    {
        [SerializeField] private CardParameterView _parameterView;//вынести
        [SerializeField] private List<Parameters> _parameters;
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private TypeMagic _typeMagic;

        public CardParameterView ParameterView => _parameterView;
        public List<Parameters> Parameters => _parameters;
        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;
        public TypeMagic MagicType => _typeMagic;
    }
}