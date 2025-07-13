using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class TipView : MonoBehaviour
    {
        [SerializeField] private LeanLocalizedText _nameTip;
        [SerializeField] private Image _tipsImage;

        public void Initialize(TipData tipsData)
        {
            _tipsImage.sprite = tipsData.Sprite;
            _nameTip.TranslationName = tipsData.TipTrnaslationName;
        }
    }
}