using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Attribute", menuName = "Create Attribute/Attribute  ", order = 51)]
    public abstract class AttributeData : ScriptableObject
    {
        [SerializeField] private CardParameterView _cardParameterView;
        [SerializeField] private List<Parameters> _parameters;
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;

        public CardParameterView CardParameterView => _cardParameterView;
        public List<Parameters> CardParameters => _parameters;
        public string NameCard => _name;
        public string Description => _description;
        public Sprite Icon => _icon;
    }
}