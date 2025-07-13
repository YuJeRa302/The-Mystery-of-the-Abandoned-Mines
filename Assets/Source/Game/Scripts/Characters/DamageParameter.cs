using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    [System.Serializable]
    public class DamageParameter
    {
        [SerializeField] private float _value;
        [SerializeField] private TypeDamageParameter _typeDamageParameter;

        public DamageParameter(float value, TypeDamageParameter typeDamageParameter)
        {
            _value = value;
            _typeDamageParameter = typeDamageParameter;
        }

        public float Value => _value;
        public TypeDamageParameter TypeDamageParameter => _typeDamageParameter;

        public void ChangeParameterValue(float value)
        {
            _value = value;
        }
    }
}