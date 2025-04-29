using System;
using UnityEngine;

[Serializable]
public class TypeParametrIcon
{
    [SerializeField] private TypeParameter _type;
    [SerializeField] private Sprite _icon;

    public TypeParameter TypeDamage => _type;
    public Sprite Icon => _icon;
}