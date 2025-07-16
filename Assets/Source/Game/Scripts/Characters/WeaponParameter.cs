using Assets.Source.Game.Scripts.Enums;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Characters
{
    [System.Serializable]

    public class WeaponParameter
    {
        [SerializeField] private float _value;
        [SerializeField] private TypeWeaponSupportiveParameter _supportiveParameter;

        public float Value => _value;
        public TypeWeaponSupportiveParameter SupportiveParameter => _supportiveParameter;
    }
}