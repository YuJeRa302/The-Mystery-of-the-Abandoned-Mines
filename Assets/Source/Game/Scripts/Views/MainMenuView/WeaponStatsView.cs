using Lean.Localization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class WeaponStatsView : MonoBehaviour
    {
        [SerializeField] private LeanLocalizedText _valueLeanLocalizedText;
        [SerializeField] private Text _valueText;
        [SerializeField] private LeanLocalizedText _nameStats;
        [SerializeField] private Image _imagePatametr;
        [SerializeField] private List<DamageParametrIcon> _damageParametrIcons;
        [SerializeField] private List<WeaponSupportiveParameterIcon> _weaponSupportiveParameterIcons;
        [SerializeField] private List<DamageSupportivePatametrIcon> _damageSupportiveParameterIcons;

        public void Initialize(string translationName, string value, bool isOneParameter)
        {
            Debug.Log(translationName);
            _nameStats.TranslationName = translationName;

            if (isOneParameter == true)
            {
                _valueLeanLocalizedText.TranslationName = value;

                RenderIconDamageType(out Sprite icon);
                _imagePatametr.sprite = icon;
            }
            else
            {
                RenderIconParametr(out Sprite icon);
                _imagePatametr.sprite = icon;
                _valueText.text = value;
            }
        }

        private void RenderIconDamageType(out Sprite icon)
        {
            icon = null;

            foreach (var parametr in _damageParametrIcons)
            {
                if (parametr.TypeDamage.ToString() == _valueLeanLocalizedText.TranslationName)
                {
                    icon = parametr.Icon;
                    return;
                }
            }
        }

        private void RenderIconParametr(out Sprite icon)
        {
            icon = null;

            foreach (var parametr in _weaponSupportiveParameterIcons)
            {
                if (parametr.TypeDamage.ToString() == _nameStats.TranslationName)
                {
                    icon = parametr.Icon;
                    return;
                }
            }

            foreach (var parametr in _damageSupportiveParameterIcons)
            {
                if (parametr.TypeDamage.ToString() == _nameStats.TranslationName)
                {
                    icon = parametr.Icon;
                    return;
                }
            }
        }
    }
}