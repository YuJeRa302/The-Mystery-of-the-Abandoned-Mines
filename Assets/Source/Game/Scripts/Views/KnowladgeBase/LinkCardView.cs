using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

public class LinkCardView : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private LeanLocalizedText _name;

    public void Initialize(Sprite icon, string name)
    {
        _icon.sprite = icon;
        _name.TranslationName = name;
    }
}