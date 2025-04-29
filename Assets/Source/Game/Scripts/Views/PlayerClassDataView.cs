using Lean.Localization;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class PlayerClassDataView : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        [SerializeField] private Image _icon;
        [SerializeField] private LeanLocalizedText _description;
        [SerializeField] private LeanLocalizedText _name;
        [SerializeField] private Button _button;

        private PlayerClassData _playerClassData;
        private IAudioPlayerService _audioPlayerService;

        public event Action<PlayerClassDataView> PlayerClassSelected;

        public PlayerClassData PlayerClassData => _playerClassData;

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnSelected);
        }

        public void Initialize(PlayerClassData playerClassData, IAudioPlayerService audioPlayerService)
        {
            _audioPlayerService = audioPlayerService;
            _playerClassData = playerClassData;
            _button.onClick.AddListener(OnSelected);
            Fill(playerClassData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _audioPlayerService.PlayOneShotButtonHoverSound();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _audioPlayerService.PlayOneShotButtonClickSound();
        }

        private void Fill(PlayerClassData playerClassData)
        {
            _icon.sprite = playerClassData.Icon;
            _description.TranslationName = playerClassData.TranslationDescription;
            _name.TranslationName = playerClassData.TranslationName;
        }

        private void OnSelected()
        {
            PlayerClassSelected?.Invoke(this);
        }
    }
}