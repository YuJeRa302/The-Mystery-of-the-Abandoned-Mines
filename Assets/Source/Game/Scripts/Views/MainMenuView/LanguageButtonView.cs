using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class LanguageButtonView : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;

        private string _languageTag;
        private IAudioPlayerService _audioPlayerService;

        public event Action<string> LanguageSelected;

        private void Awake()
        {
            _button.onClick.AddListener(OnSelectLanguage);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnSelectLanguage);
        }

        public void Initialize(LanguageButtonData languageButtonData, IAudioPlayerService audioPlayerService)
        {
            _image.sprite = languageButtonData.IconLanguage;
            _languageTag = languageButtonData.NameLanguage;
            _audioPlayerService = audioPlayerService;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _audioPlayerService?.PlayOneShotButtonHoverSound();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _audioPlayerService.PlayOneShotButtonClickSound();
        }

        private void OnSelectLanguage()
        {
            LanguageSelected?.Invoke(_languageTag);
        }
    }
}