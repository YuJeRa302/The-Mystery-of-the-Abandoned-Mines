using Assets.Source.Game.Scripts.ScriptableObjects;
using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class PlayerClassKnowledgeView : KnowledgeView
    {
        [SerializeField] private Image _icon;
        [SerializeField] private LeanLocalizedText _className;
        [SerializeField] private LeanLocalizedText _classDescription;
        [SerializeField] private LeanLocalizedText _classStrengths;
        [SerializeField] private Image _baseWeaponIcon;
        [SerializeField] private Transform _classAbilityIconContainer;
        [SerializeField] private Image _imagePrefab;

        public void Initialize(PlayerClassData classData)
        {
            _className.TranslationName = classData.TranslationName;
            _icon.sprite = classData.Icon;
            _classDescription.TranslationName = classData.TranslationDescription;
            _classStrengths.TranslationName = classData.TranslationStrengths;
            _baseWeaponIcon.sprite = classData.BaseWeapon.Icon;

            foreach (var ability in classData.ClassAbilityDatas)
            {
                Image image = Instantiate(_imagePrefab, _classAbilityIconContainer);
                image.sprite = ability.Icon;
            }
        }
    }
}