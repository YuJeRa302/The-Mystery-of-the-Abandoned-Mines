using Assets.Source.Game.Scripts.Models;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using Assets.Source.Game.Scripts.Views;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.GamePanels
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

        private void OnDestroy()
        {
            ClearLanguageButton();
            RemoveListeners();
        }

        public override void Initialize(GamePanelsModel gamePanelsModel)
        {
            _ambientSlider.value = gamePanelsModel.AmbientVolumeValue;
            _sfxSlider.value = gamePanelsModel.SfxVolumeValue;
            _muteToggle.isOn = gamePanelsModel.IsMuted;
            _audioPlayerService = gamePanelsModel.AudioPlayer;
            AddListeners();
            ClearLanguageButton();
            CreateLanguageButton();
            base.Initialize(gamePanelsModel);
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
            _exitButton.onClick.AddListener(CloseGame);
            _ambientSlider.onValueChanged.AddListener(OnAmbientValueChanged);
            _sfxSlider.onValueChanged.AddListener(OnSfxValueChanged);
            _muteToggle.onValueChanged.AddListener(OnMuteValueChanged);
        }

        private void RemoveListeners()
        {
            _exitButton.onClick.RemoveListener(CloseGame);
            _openButton.onClick.RemoveListener(Open);
            _resumeButton.onClick.RemoveListener(Close);
            _ambientSlider.onValueChanged.RemoveListener(OnAmbientValueChanged);
            _sfxSlider.onValueChanged.RemoveListener(OnSfxValueChanged);
            _muteToggle.onValueChanged.RemoveListener(OnMuteValueChanged);
        }

        private void FillGameParameters()
        {
            _playerHealth.text = GamePanelsModel.GetPlayer().CurrentHealth.ToString();
            _playerDamage.text = GamePanelsModel.GetPlayer().DamageSource.Damage.ToString();
            _coins.text = GamePanelsModel.GetPlayer().Coins.ToString();
            _rerollPoints.text = GamePanelsModel.GetPlayer().RerollPoints.ToString();
            _currentRoomLevel.text = GamePanelsModel.GetCurrentRoomLevel().ToString();
            _killCount.text = GamePanelsModel.GetPlayer().KillCount.ToString();
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

        private void ClearLanguageButton()
        {
            if (_languageButtonViews.Count == 0)
                return;

            foreach (LanguageButtonView view in _languageButtonViews)
            {
                view.LanguageSelected -= OnLanguageChanged;
                Destroy(view.gameObject);
            }

            _languageButtonViews.Clear();
        }

        private void OnLanguageChanged(string value) => GamePanelsModel.SetLanguage(value);
        private void OnAmbientValueChanged(float value) => GamePanelsModel.SetAmbientVolume(value);
        private void OnSfxValueChanged(float value) => GamePanelsModel.SetSfxVolume(value);
        private void OnMuteValueChanged(bool value) => GamePanelsModel.SetMute(value);
    }
}