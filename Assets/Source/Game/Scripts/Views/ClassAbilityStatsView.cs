using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

public class ClassAbilityStatsView : MonoBehaviour
{
    [SerializeField] private LeanLocalizedText _valueLeanLocalizedText;
    [SerializeField] private LeanLocalizedText _nameStats;
    [SerializeField] private Text _valueCurrentLvlText;
    [SerializeField] private Text _valueNextLvlText;
    [SerializeField] private Image _haveNextLvlImage;

    public void Initialize(string translationName, string valueCurrentLvl, string valueNextLvl)
    {
        _nameStats.TranslationName = translationName;
        _valueCurrentLvlText.text = valueCurrentLvl;
        _valueNextLvlText.text = valueNextLvl;

        if (_valueNextLvlText.text == string.Empty)
        {
            _haveNextLvlImage.gameObject.SetActive(false);
        }
    }
}