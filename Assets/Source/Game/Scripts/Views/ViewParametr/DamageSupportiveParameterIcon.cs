using Assets.Source.Game.Scripts.Enums;
using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Views
{
    [Serializable]
    public class DamageSupportiveParameterIcon
    {
        [SerializeField] private TypeDamageParameter _typeDamage;
        [SerializeField] private Sprite _icon;

        public TypeDamageParameter TypeDamage => _typeDamage;
        public Sprite Icon => _icon;
    }
}