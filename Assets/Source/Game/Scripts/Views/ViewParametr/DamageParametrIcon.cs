using Assets.Source.Game.Scripts.Enums;
using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Views
{
    [Serializable]
    public class DamageParametrIcon
    {
        [SerializeField] private TypeDamage _typeDamage;
        [SerializeField] private Sprite _icon;

        public TypeDamage TypeDamage => _typeDamage;
        public Sprite Icon => _icon;
    }
}