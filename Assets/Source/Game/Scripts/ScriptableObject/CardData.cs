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

        public int Id => _id;
        public Color[] TypeCardColor => _typeCardColor;
        public AttributeData AttributeData => _attributeData;
        public TypeCardParameter TypeCardParameter => _typeCardParameter;
    }
}