using System;
using UnityEngine;

[Serializable]
public class WeaponSupportiveParameterIcon
{
    [SerializeField] private TypeWeaponSupportiveParameter _typeDamage;
    [SerializeField] private Sprite _icon;

    public TypeWeaponSupportiveParameter TypeDamage => _typeDamage;
    public Sprite Icon => _icon;
}