using Assets.Source.Game.Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Create Card", order = 51)]
    public class CardData : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private Color[] _typeCardColor;
        [SerializeField] private TypeCardParameter _typeCardParameter;
        [SerializeField] private AttributeData _attributeData;
        [SerializeField] private List<AttributeData> _suppurtiveData;

        public int Id => _id;
        public Color[] TypeCardColor => _typeCardColor;
        public AttributeData AttributeData => _attributeData;
        public TypeCardParameter TypeCardParameter => _typeCardParameter;
        public List<AttributeData> SupportiveData => _suppurtiveData;
    }
}