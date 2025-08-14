using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Card
{
    public abstract class CardParameterView : MonoBehaviour
    {
        [SerializeField] private Text _parameterValue;
        [SerializeField] private LeanLocalizedText _parameterName;

        public void Initialize(int parameterValue, string typeParameter)
        {
            _parameterValue.text = parameterValue.ToString();
            _parameterName.TranslationName = typeParameter.ToString();
        }
    }
}