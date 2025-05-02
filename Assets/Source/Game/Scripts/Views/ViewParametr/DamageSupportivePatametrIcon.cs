using System;
using UnityEngine;

[Serializable]
public class DamageSupportivePatametrIcon
{
    [SerializeField] private TypeDamageParameter _typeDamage;
    [SerializeField] private Sprite _icon;

    public TypeDamageParameter TypeDamage => _typeDamage;
    public Sprite Icon => _icon;
}