using Assets.Source.Game.Scripts.Enums;
using System;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Views
{
    [Serializable]
    public class TypeParametrIcon
    {
        [SerializeField] private TypeParameter _type;
        [SerializeField] private Sprite _icon;

        public TypeParameter TypeDamage => _type;
        public Sprite Icon => _icon;
    }
}