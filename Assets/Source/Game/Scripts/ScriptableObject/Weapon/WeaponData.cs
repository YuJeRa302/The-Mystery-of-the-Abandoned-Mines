using Assets.Source.Game.Scripts.Characters;
using Assets.Source.Game.Scripts.Enums;
using Assets.Source.Game.Scripts.Items;
using Assets.Source.Game.Scripts.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Create Weapon/Weapon", order = 51)]
    public class WeaponData : ScriptableObject, IIdData
    {
        [SerializeField] private int _id;
        [SerializeField] private int _tier;
        [SerializeField] private Color[] _tierColor;
        [SerializeField] private TypePlayerClass _targetClass;
        [SerializeField] private WeaponPrefab _weaponPrefab;
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
        public WeaponPrefab WeaponPrefab => _weaponPrefab;
        public DamageSource DamageSource => _damageSource;
        public List<WeaponParameter> WeaponParameters => _weaponParameters;
    }
}