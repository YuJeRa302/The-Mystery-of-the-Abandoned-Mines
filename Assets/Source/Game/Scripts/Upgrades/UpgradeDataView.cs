using Assets.Source.Game.Scripts.Models;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.ViewModels;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Upgrades
{
    public class UpgradeDataView : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        private readonly int _minValue = 0;

        [SerializeField] private Image _iconStats;
        [SerializeField] private Image[] _upgradeImages;
        [SerializeField] private Sprite _upgradeSprite;
        [SerializeField] private Button _button;

        private UpgradeState _upgradeState;
        private UpgradeData _upgradeData;
        private IAudioPlayerService _audioPlayerService;
        private CompositeDisposable _disposables = new();

        public event Action<UpgradeDataView> StatsSelected;

        public UpgradeState UpgradeState => _upgradeState;
        public UpgradeData UpgradeData => _upgradeData;

        private void OnDestroy()
        {
            if (_disposables != null)
                _disposables.Dispose();

            _button.onClick.RemoveListener(OnSelected);
        }

        public void Initialize(UpgradeData upgradeData,
            UpgradeState upgradeState,
            IAudioPlayerService audioPlayerService)
        {
            _audioPlayerService = audioPlayerService;
            _upgradeData = upgradeData;
            _upgradeState = upgradeState;
            _button.onClick.AddListener(OnSelected);

            MessageBroker.Default
                .Receive<M_StatsUpgraded>()
                .Subscribe(m => OnStateUpgraded(m.UpgradeState))
                .AddTo(_disposables);

            MessageBroker.Default
                .Receive<M_StatsReseted>()
                .Subscribe(m => OnResetState())
                .AddTo(_disposables);

            Fill(upgradeData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _audioPlayerService.PlayOneShotButtonHoverSound();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _audioPlayerService.PlayOneShotButtonClickSound();
        }

        private void OnResetState()
        {
            if (_upgradeState == null)
                return;

            _upgradeState.ChangeCurrentLevel(_minValue);
        }

        private void OnStateUpgraded(UpgradeState upgradeState)
        {
            if (_upgradeState.Id == upgradeState.Id)
                _upgradeState = upgradeState;

            Fill(_upgradeData);
        }

        private void Fill(UpgradeData upgradeData)
        {
            _iconStats.sprite = upgradeData.Icon;

            if (_upgradeState == null)
                return;

            for (int index = 0; index < _upgradeState.CurrentLevel; index++)
            {
                _upgradeImages[index].sprite = _upgradeSprite;
            }
        }

        private void OnSelected()
        {
            StatsSelected?.Invoke(this);
        }
    }
}