using Assets.Source.Game.Scripts;
using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

public class PlayerClassKnowladgeView : KnowladgeView
{
    [SerializeField] private Image _icon;
    [SerializeField] private LeanLocalizedText _className;
    [SerializeField] private LeanLocalizedText _classDiscription;
    [SerializeField] private LeanLocalizedText _classStrengths;
    [SerializeField] private Image _baseWeaponIcon;
    [SerializeField] private Transform _classAbilityIconContainer;
    [SerializeField] private Image _imagePrafab;

    public void Initialize(PlayerClassData classData)
    {
        _className.TranslationName = classData.TranslationName;
        _icon.sprite = classData.Icon;
        _classDiscription.TranslationName = classData.TranslationDescription;
        _classStrengths.TranslationName = classData.TranslationStrengths;
        _baseWeaponIcon.sprite = classData.BaseWeapon.Icon;
        
        foreach(var ability in classData.ClassAbilityDatas)
        {
            Image image = Instantiate(_imagePrafab, _classAbilityIconContainer);
            image.sprite = ability.Icon;
        }
    }
}