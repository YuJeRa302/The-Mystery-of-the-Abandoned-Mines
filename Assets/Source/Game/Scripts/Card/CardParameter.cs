using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Card
{
    [System.Serializable]
    public class CardParameter
    {
        [SerializeField] private int _value;
        [SerializeField] private TypeParameter _typeParameter;

        public int Value => _value;
        public TypeParameter TypeParameter => _typeParameter;
    }
}