using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

public class TranslationText : MonoBehaviour
{
    [SerializeField] private Text _text;
    [SerializeField] private LeanLocalizedText _leanLocalizedText;

    private string _key;

    private void Awake()
    {
        _key = _text.text;
    }

    private void OnEnable()
    {
        _leanLocalizedText.TranslationName = _key;
    }
}