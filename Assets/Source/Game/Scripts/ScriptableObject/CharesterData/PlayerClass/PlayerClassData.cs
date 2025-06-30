using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Class", menuName = "Create Player Class", order = 51)]
    public class PlayerClassData : ScriptableObject
    {
        [SerializeField] private int _id;
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _translationName;
        [SerializeField] private string _translationDescription;
        [SerializeField] private string _translationStrengths;
        [SerializeField] private TypePlayerClass _className;
        [SerializeField] private TypeAttackRange _typeAttackRange;
        [SerializeField] private RuntimeAnimatorController _animatorController;
        [Space(10)]
        [SerializeField] private List<ClassAbilityData> _classAbilityDatas;
        [SerializeField] private WeaponData _baseWeapon;
        [Space(10)]
        [SerializeField] private AudioClip _attackEffect;

        public int Id => _id;
        public List<ClassAbilityData> ClassAbilityDatas => _classAbilityDatas;
        public string TranslationDescription => _translationDescription;
        public string TranslationName => _translationName;
        public Sprite Icon => _icon;
        public RuntimeAnimatorController AnimatorController => _animatorController;
        public TypePlayerClass TypePlayerClass => _className;
        public WeaponData BaseWeapon => _baseWeapon;
        public string TranslationStrengths => _translationStrengths;
        public TypeAttackRange TypeAttackRange => _typeAttackRange;
        public AudioClip AttackEffect => _attackEffect;
    }
}