using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class ClassAbilityDataView : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        private readonly int _minValue = 0;

        [SerializeField] private Image _iconAbility;
        [SerializeField] private Image[] _upgradeImages;
        [SerializeField] private Sprite _upgradeSprite;
        [SerializeField] private Button _button;

        private ClassAbilityViewModel _classAbilityViewModel;
        private IAudioPlayerService _audioPlayerService;

        public event Action<ClassAbilityDataView> AbilitySelected;

        public ClassAbilityState ClassAbilityState { get; private set; }
        public ClassAbilityData ClassAbilityData { get; private set; }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnSelected);
            _classAbilityViewModel.InvokedAbilityUpgrade -= OnAbilityUpgraded;
            _classAbilityViewModel.InvokedAbilityReset -= OnResetState;
        }

        public void Initialize(ClassAbilityData classAbilityData, 
            ClassAbilityState classAbilityState, 
            ClassAbilityViewModel classAbilityViewModel, 
            IAudioPlayerService audioPlayerService)
        {
            _audioPlayerService = audioPlayerService;
            _classAbilityViewModel = classAbilityViewModel;
            ClassAbilityState = classAbilityState;
            ClassAbilityData = classAbilityData;
            _button.onClick.AddListener(OnSelected);
            _classAbilityViewModel.InvokedAbilityUpgrade += OnAbilityUpgraded;
            _classAbilityViewModel.InvokedAbilityReset += OnResetState;
            Fill(ClassAbilityData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _audioPlayerService.PlayOneShotButtonHoverSound();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _audioPlayerService.PlayOneShotButtonClickSound();
        }

        private void OnResetState(PlayerClassData playerClassData)
        {
            if (ClassAbilityState == null)
                return;

            ClassAbilityState.CurrentLevel = _minValue;
        }

        private void OnAbilityUpgraded(ClassAbilityState classAbilityState)
        {
            if (ClassAbilityState.Id == classAbilityState.Id)
                ClassAbilityState = classAbilityState;

            Fill(ClassAbilityData);
        }

        private void Fill(ClassAbilityData classAbilityData)
        {
            _iconAbility.sprite = classAbilityData.Icon;

            if (ClassAbilityState == null)
                return;

            for (int index = 0; index < ClassAbilityState.CurrentLevel; index++)
            {
                _upgradeImages[index].sprite = _upgradeSprite;
            }
        }

        private void OnSelected()
        {
            AbilitySelected?.Invoke(this);
        }
    }
}