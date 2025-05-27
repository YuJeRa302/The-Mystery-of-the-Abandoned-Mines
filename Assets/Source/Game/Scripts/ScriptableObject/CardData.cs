using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Create Card", order = 51)]
    public class CardData : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private Color[] _typeCardColor;
        [SerializeField] private TypeCardParameter _typeCardParameter;
        [SerializeField] private AttributeData _attributeData;
        [SerializeField] private LegendaryAbilityData _legendaryAbilityData;
        [SerializeField] private List<AttributeData> _suppurtivData;

        public int Id => _id;
        public Color[] TypeCardColor => _typeCardColor;
        public AttributeData AttributeData => _attributeData;
        //public LegendaryAbilityData LegendaryAbilityData => _legendaryAbilityData;
        public TypeCardParameter TypeCardParameter => _typeCardParameter;
        public List<AttributeData> SuppurtivData => _suppurtivData;
    }
}