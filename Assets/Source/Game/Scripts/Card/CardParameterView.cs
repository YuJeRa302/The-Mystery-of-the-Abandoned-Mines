using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public abstract class CardParameterView : MonoBehaviour
    {
        [SerializeField] private Text _parameterValue;
        [SerializeField] private LeanLocalizedText _parameterName;

        public void Initialize(int parameterValue, TypeParameter typeParameter)
        {
            _parameterValue.text = parameterValue.ToString();
            _parameterName.TranslationName = typeParameter.ToString();
        }
    }
}