using Lean.Localization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassAbilityStatsView : MonoBehaviour
{
    [SerializeField] private LeanLocalizedText _nameStats;
    [SerializeField] private Text _valueCurrentLvlText;
    [SerializeField] private Text _valueNextLvlText;
    [SerializeField] private Image _haveNextLvlImage;
    [SerializeField] private Image _iconParametr;
    [SerializeField] private List<TypeParametrIcon> _parametrIcons;

    public void Initialize(string translationName, string valueCurrentLvl, string valueNextLvl)
    {
        _nameStats.TranslationName = translationName;
        _valueCurrentLvlText.text = valueCurrentLvl;
        _valueNextLvlText.text = valueNextLvl;
        RenderIcon();

        if (_valueNextLvlText.text == string.Empty)
        {
            _haveNextLvlImage.gameObject.SetActive(false);
        }
    }

    private void RenderIcon()
    {
        foreach (var parametr in _parametrIcons)
        {
            if (parametr.TypeDamage.ToString() == _nameStats.TranslationName)
            {
                _iconParametr.sprite = parametr.Icon;
                return;
            }
        }
    }
}