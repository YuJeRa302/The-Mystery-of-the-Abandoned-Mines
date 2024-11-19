using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class UpgradeDataView : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        private readonly int _minValue = 0;

        [SerializeField] private Image _iconStats;
        [SerializeField] private Image[] _upgradeImages;
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _upgradeSprite;
        [SerializeField] private Button _button;

        private UpgradeState _upgradeState;
        private UpgradeData _upgradeData;
        private UpgradeMenuLoad _upgradeMenuLoad;
        private AudioClip _hover;
        private AudioClip _click;
        private AudioSource _audioSource;

        public event Action<UpgradeDataView> StatsSelected;

        public UpgradeState UpgradeState => _upgradeState;
        public UpgradeData UpgradeData => _upgradeData;

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnSelected);
            _upgradeMenuLoad.StatsUpgraded -= OnStateChanged;
            _upgradeMenuLoad.StatsReseted -= OnResetState;
        }

        public void Initialize(UpgradeData upgradeData, UpgradeState upgradeState, UpgradeMenuLoad upgradeMenuLoad, AudioSource audioSource, AudioClip click, AudioClip hover)
        {
            _audioSource = audioSource;
            _hover = hover;
            _click = click;
            _upgradeMenuLoad = upgradeMenuLoad;
            _upgradeData = upgradeData;
            _upgradeState = upgradeState;
            _button.onClick.AddListener(OnSelected);
            _upgradeMenuLoad.StatsUpgraded += OnStateChanged;
            _upgradeMenuLoad.StatsReseted += OnResetState;
            Fill(upgradeData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _audioSource.PlayOneShot(_hover);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _audioSource.PlayOneShot(_click);
        }

        private void OnResetState()
        {
            if (_upgradeState == null)
                return;

            _upgradeState.CurrentLevel = _minValue;
        }

        private void OnStateChanged(UpgradeState upgradeState)
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