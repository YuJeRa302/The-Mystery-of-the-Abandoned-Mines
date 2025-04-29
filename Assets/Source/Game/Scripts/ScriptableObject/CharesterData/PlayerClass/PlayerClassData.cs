using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.Source.Game.Scripts
{
    [CreateAssetMenu(fileName = "New Class", menuName = "Create Player Class", order = 51)]
    public class PlayerClassData : ScriptableObject
    {
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _translationName;
        [SerializeField] private string _translationDescription;
        [SerializeField] private string _translationStrengths;
        [SerializeField] private TypePlayerClass _className;
        [SerializeField] private AnimatorController _animatorController;
        [Space(10)]
        [SerializeField] private List<ClassAbilityData> _classAbilityDatas;
        [SerializeField] private WeaponData _baseWeapon;

        public List<ClassAbilityData> ClassAbilityDatas => _classAbilityDatas;
        public string TranslationDescription => _translationDescription;
        public string TranslationName => _translationName;
        public Sprite Icon => _icon;
        public AnimatorController AnimatorController => _animatorController;
        public TypePlayerClass TypePlayerClass => _className;
        public WeaponData BaseWeapon => _baseWeapon;
        public string TranslationStrengths => _translationStrengths;
    }
}