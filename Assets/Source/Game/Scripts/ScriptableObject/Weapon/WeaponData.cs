using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Create Weapon/Weapon", order = 51)]
public class WeaponData : ScriptableObject
{
    [SerializeField] private int _id;
    [SerializeField] private int _tier;
    [SerializeField] private Color[] _tierColor;
    [SerializeField] private TypePlayerClass _targetClass;
    [SerializeField] private WeponPrefab _weaponPrefab;
    [SerializeField] private string _translationName;
    [SerializeField] private Sprite _icon;
    [SerializeField] private DamageSource _damageSource;
    [SerializeField] private List<WeaponParameter> _weaponParameters;

    public Color TierColor => _tierColor[_tier];
    public int Tier => _tier;
    public int Id => _id;
    public string TranslationName => _translationName;
    public Sprite Icon => _icon;
    public TypePlayerClass TypePlayerClass => _targetClass;
    public WeponPrefab WeaponPrefab => _weaponPrefab;
    public DamageSource DamageSource => _damageSource;
    public List<WeaponParameter> WeaponParameters => _weaponParameters;
}