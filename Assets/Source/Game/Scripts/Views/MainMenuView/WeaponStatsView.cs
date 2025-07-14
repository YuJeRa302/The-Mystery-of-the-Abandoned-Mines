using Lean.Localization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class WeaponStatsView : MonoBehaviour
    {
        private readonly string _damageParameterName = "Damage";

        [SerializeField] private LeanLocalizedText _valueLeanLocalizedText;
        [SerializeField] private Text _valueText;
        [SerializeField] private LeanLocalizedText _nameStats;
        [SerializeField] private Image _imageParameter;
        [SerializeField] private Sprite _damageIcon;
        [SerializeField] private List<DamageParameterIcon> _damageParameterIcons;
        [SerializeField] private List<WeaponSupportiveParameterIcon> _weaponSupportiveParameterIcons;
        [SerializeField] private List<DamageSupportiveParameterIcon> _damageSupportiveParameterIcons;

        public void Initialize(string translationName, string value, bool isOneParameter)
        {
            _nameStats.TranslationName = translationName;

            if (isOneParameter == true)
            {
                _valueLeanLocalizedText.TranslationName = value;

                RenderIconDamageType(out Sprite icon);
                _imageParameter.sprite = icon;
            }
            else
            {
                if (translationName == _damageParameterName)
                {
                    _imageParameter.sprite = _damageIcon;
                }
                else
                {
                    RenderIconParameter(out Sprite icon);
                    _imageParameter.sprite = icon;
                }

                _valueText.text = value;
            }
        }

        private void RenderIconDamageType(out Sprite icon)
        {
            icon = null;

            foreach (var parameter in _damageParameterIcons)
            {
                if (parameter.TypeDamage.ToString() == _valueLeanLocalizedText.TranslationName)
                {
                    icon = parameter.Icon;
                    return;
                }
            }
        }

        private void RenderIconParameter(out Sprite icon)
        {
            icon = null;

            foreach (var parameter in _weaponSupportiveParameterIcons)
            {
                if (parameter.TypeDamage.ToString() == _nameStats.TranslationName)
                {
                    icon = parameter.Icon;
                    return;
                }
            }

            foreach (var parameter in _damageSupportiveParameterIcons)
            {
                if (parameter.TypeDamage.ToString() == _nameStats.TranslationName)
                {
                    icon = parameter.Icon;
                    return;
                }
            }
        }
    }
}