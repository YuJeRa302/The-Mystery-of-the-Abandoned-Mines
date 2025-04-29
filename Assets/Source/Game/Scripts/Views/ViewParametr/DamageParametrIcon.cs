using System;
using UnityEngine;

[Serializable]
public class DamageParametrIcon 
{
    [SerializeField] private TypeDamage _typeDamage;
    [SerializeField] private Sprite _icon;

    public TypeDamage TypeDamage => _typeDamage;
    public Sprite Icon => _icon;
}