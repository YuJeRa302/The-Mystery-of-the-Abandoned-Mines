using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Attribute", menuName = "Create Attribute/Attribute  ", order = 51)]
    public abstract class AttributeData : ScriptableObject
    {
        [SerializeField] protected CardParameterView _parameterView;
        [SerializeField] protected List<Parameters> _parameters;
        [SerializeField] protected string _name;
        [SerializeField] protected string _description;
        [SerializeField] protected Sprite _icon;
        [SerializeField] protected TypeMagic _typeMagic;

        public CardParameterView ParameterView => _parameterView;
        public List<Parameters> Parameters => _parameters;
        public string Name => _name;
        public string Description => _description;
        public Sprite Icon => _icon;
        public TypeMagic MagicType => _typeMagic;
    }
}