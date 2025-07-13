using Assets.Source.Game.Scripts.Enums;
using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Views
{
    [Serializable]
    public class WeaponSupportiveParameterIcon
    {
        [SerializeField] private TypeWeaponSupportiveParameter _typeDamage;
        [SerializeField] private Sprite _icon;

        public TypeWeaponSupportiveParameter TypeDamage => _typeDamage;
        public Sprite Icon => _icon;
    }
}