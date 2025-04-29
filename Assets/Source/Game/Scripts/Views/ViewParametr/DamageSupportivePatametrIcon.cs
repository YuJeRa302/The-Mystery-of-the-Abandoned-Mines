using System;
using UnityEngine;

[Serializable]
public class DamageSupportivePatametrIcon
{
    [SerializeField] private TypeSupportivePatametr _typeDamage;
    [SerializeField] private Sprite _icon;

    public TypeSupportivePatametr TypeDamage => _typeDamage;
    public Sprite Icon => _icon;
}