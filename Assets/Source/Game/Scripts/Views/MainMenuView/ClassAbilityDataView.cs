using Assets.Source.Game.Scripts.Models;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.States;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class ClassAbilityDataView : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        private readonly int _minValue = 0;

        [SerializeField] private Image _iconAbility;
        [SerializeField] private Image[] _upgradeImages;
        [SerializeField] private Sprite _upgradeSprite;
        [SerializeField] private Button _button;

        private IAudioPlayerService _audioPlayerService;
        private CompositeDisposable _disposables = new();

        public event Action<ClassAbilityDataView> AbilitySelected;

        public ClassAbilityState ClassAbilityState { get; private set; }
        public ClassAbilityData ClassAbilityData { get; private set; }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnSelected);
        }

        public void Initialize(ClassAbilityData classAbilityData,
            ClassAbilityState classAbilityState,
            IAudioPlayerService audioPlayerService)
        {
            _audioPlayerService = audioPlayerService;
            ClassAbilityState = classAbilityState;
            ClassAbilityData = classAbilityData;
            _button.onClick.AddListener(OnSelected);

            MessageBroker.Default
                .Receive<M_AbilityUpgraded>()
                .Subscribe(m => OnAbilityUpgraded(m.ClassAbilityState))
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_AbilityReseted>()
                .Subscribe(m => OnResetState(m.PlayerClassData))
                .AddTo(_disposables);

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

            ClassAbilityState.ChangeCurrentLevel(_minValue);
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