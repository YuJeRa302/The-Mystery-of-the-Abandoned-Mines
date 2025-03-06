using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class WeaponStatsView : MonoBehaviour
    {
        [SerializeField] private LeanLocalizedText _valueLeanLocalizedText;
        [SerializeField] private Text _valueText;
        [SerializeField] private LeanLocalizedText _nameStats;

        public void Initialize(string translationName, string value, bool isOneParameter)
        {
            _nameStats.TranslationName = translationName;

            if (isOneParameter == true)
                _valueLeanLocalizedText.TranslationName = value;
            else
                _valueText.text = value;
        }
    }
}