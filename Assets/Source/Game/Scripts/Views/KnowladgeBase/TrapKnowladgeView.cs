using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

public class TrapKnowladgeView : KnowladgeView
{
    [SerializeField] Image _icon;
    [SerializeField] private LeanLocalizedText _name;
    [SerializeField] private LeanLocalizedText _description;

    public void Initialize(TrapData trapData)
    {
        _icon.sprite = trapData.Icon;
        _name.TranslationName = trapData.Name;
        _description.TranslationName = trapData.Description;
    }
}