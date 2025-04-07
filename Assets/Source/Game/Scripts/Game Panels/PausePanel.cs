using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts
{
    public class PausePanel : GamePanelsView
    {
        [SerializeField] private Button _openButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _resumeButton;
        [Space(20)]
        [SerializeField] private Slider _ambientSlider;
        [SerializeField] private Slider _sfxSlider;
        [Space(20)]
        [SerializeField] private Toggle _muteToggle;
        [SerializeField] private LanguageButtonData[] _languageButtonData;
        [SerializeField] private LanguageButtonView _languageButtonView;
        [SerializeField] private Transform _buttonsContainer;
        [Space(20)]
        [SerializeField] private Text _playerHealth;
        [SerializeField] private Text _playerDamage;
        [SerializeField] private Text _coins;
        [SerializeField] private Text _rerollPoints;
        [Space(10)]
        [SerializeField] private Text _currentRoomLevel;
        [SerializeField] private Text _killCount;

        private List<LanguageButtonView> _languageButtonViews = new ();
        private IAudioPlayerService _audioPlayerService;

        public event Action ExitButtonClicked;

        private void Awake()
        {
            _exitButton.onClick.AddListener(ClickButtonExit);
        }

        private void ClickButtonExit()
        {
            ExitButtonClicked?.Invoke();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        public override void Initialize(GamePanelsViewModel gamePanelsViewModel)
        {
            AddListeners();
            CreateLanguageButton();
            base.Initialize(gamePanelsViewModel);
        }

        protected override void Open()
        {
            base.Open();
            FillGameParameters();
        }

        private void AddListeners()
        {
            _openButton.onClick.AddListener(Open);
            _resumeButton.onClick.AddListener(Close);
            _ambientSlider.onValueChanged.AddListener(OnAmbientValueChanged);
            _sfxSlider.onValueChanged.AddListener(OnSfxValueChanged);
            _muteToggle.onValueChanged.AddListener(OnMuteValueChanged);
        }

        private void RemoveListeners()
        {
            _openButton.onClick.RemoveListener(Open);
            _resumeButton.onClick.RemoveListener(Close);
            _ambientSlider.onValueChanged.RemoveListener(OnAmbientValueChanged);
            _sfxSlider.onValueChanged.RemoveListener(OnSfxValueChanged);
            _muteToggle.onValueChanged.RemoveListener(OnMuteValueChanged);
        }

        private void FillGameParameters() 
        {
            _playerHealth.text = GamePanelsViewModel.GetPlayer().PlayerHealth.CurrentHealth.ToString();
            _playerDamage.text = GamePanelsViewModel.GetPlayer().PlayerStats.Damage.ToString();
            _coins.text = GamePanelsViewModel.GetPlayer().PlayerWallet.CurrentCoins.ToString();
            _rerollPoints.text = GamePanelsViewModel.GetPlayer().PlayerStats.RerollPoints.ToString();
            _currentRoomLevel.text = GamePanelsViewModel.GetCurrentRoomLevel().ToString();
            _killCount.text = GamePanelsViewModel.GetPlayer().PlayerStats.CountKillEnemy.ToString();
        }

        private void CreateLanguageButton() 
        {
            foreach (LanguageButtonData languageButton in _languageButtonData)
            {
                LanguageButtonView view = Instantiate(_languageButtonView, _buttonsContainer);
                _languageButtonViews.Add(view);
                view.Initialize(languageButton, _audioPlayerService);
                view.LanguageSelected += OnLanguageChanged;
            }
        }

        private void OnLanguageChanged(string value) => GamePanelsViewModel.SetLanguage(value);
        private void OnAmbientValueChanged(float value) => GamePanelsViewModel.SetAmbientVolume(value);
        private void OnSfxValueChanged(float value) => GamePanelsViewModel.SetSfxVolume(value);
        private void OnMuteValueChanged(bool value) => GamePanelsViewModel.SetMuteStatus(value);
    }
}