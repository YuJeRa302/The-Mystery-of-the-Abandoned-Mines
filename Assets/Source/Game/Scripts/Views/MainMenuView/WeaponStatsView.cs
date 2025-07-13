using Lean.Localization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class WeaponStatsView : MonoBehaviour
    {
        private readonly string _damageParametrName = "Damage";

        [SerializeField] private LeanLocalizedText _valueLeanLocalizedText;
        [SerializeField] private Text _valueText;
        [SerializeField] private LeanLocalizedText _nameStats;
        [SerializeField] private Image _imagePatametr;
        [SerializeField] private Sprite _damageIcon;
        [SerializeField] private List<DamageParametrIcon> _damageParametrIcons;
        [SerializeField] private List<WeaponSupportiveParameterIcon> _weaponSupportiveParameterIcons;
        [SerializeField] private List<DamageSupportivePatametrIcon> _damageSupportiveParameterIcons;

        public void Initialize(string translationName, string value, bool isOneParameter)
        {
            _nameStats.TranslationName = translationName;

            if (isOneParameter == true)
            {
                _valueLeanLocalizedText.TranslationName = value;

                RenderIconDamageType(out Sprite icon);
                _imagePatametr.sprite = icon;
            }
            else
            {
                if (translationName == _damageParametrName)
                {
                    _imagePatametr.sprite = _damageIcon;
                }
                else
                {
                    RenderIconParametr(out Sprite icon);
                    _imagePatametr.sprite = icon;
                }

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