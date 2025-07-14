using Lean.Localization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class ClassAbilityStatsView : MonoBehaviour
    {
        [SerializeField] private LeanLocalizedText _nameStats;
        [SerializeField] private Text _valueCurrentLvlText;
        [SerializeField] private Text _valueNextLvlText;
        [SerializeField] private Image _haveNextLvlImage;
        [SerializeField] private Image _iconParameter;
        [SerializeField] private List<TypeParameterIcon> _parameterIcons;

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
            foreach (var parameter in _parameterIcons)
            {
                if (parameter.TypeDamage.ToString() == _nameStats.TranslationName)
                {
                    _iconParameter.sprite = parameter.Icon;
                    return;
                }
            }
        }
    }
}