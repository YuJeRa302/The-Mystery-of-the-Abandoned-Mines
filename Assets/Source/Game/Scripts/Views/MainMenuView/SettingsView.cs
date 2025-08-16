using Assets.Source.Game.Scripts.Models;
using Assets.Source.Game.Scripts.ScriptableObjects;
using Assets.Source.Game.Scripts.Services;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.Game.Scripts.Views
{
    public class SettingsView : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Slider _ambientSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Toggle _muteToggle;
        [SerializeField] private LanguageButtonData[] _languageButtonData;
        [SerializeField] private LanguageButtonView _languageButtonView;
        [SerializeField] private Transform _buttonsContainer;

        private SettingsModel _settingsModel;
        private List<LanguageButtonView> _languageButtonViews = new();
        private CompositeDisposable _disposables = new();
        private IAudioPlayerService _audioPlayerService;

        private void OnDestroy()
        {
            ClearLanguageButtons();
            RemoveListeners();
        }

        public void Initialize(SettingsModel settingsModel, IAudioPlayerService audioPlayerService)
        {
            _settingsModel = settingsModel;
            _audioPlayerService = audioPlayerService;
            _ambientSlider.value = _settingsModel.AmbientVolumeValue;
            _sfxSlider.value = _settingsModel.SfxVolumeValue;
            _muteToggle.isOn = _settingsModel.IsMuted;
            AddListeners();
            ClearLanguageButtons();
            CreateLanguageButtons();
            gameObject.SetActive(false);
        }

        private void CreateLanguageButtons()
        {
            foreach (LanguageButtonData languageButton in _languageButtonData)
            {
                LanguageButtonView view = Instantiate(_languageButtonView, _buttonsContainer);
                _languageButtonViews.Add(view);
                view.Initialize(languageButton, _audioPlayerService);
                view.LanguageSelected += OnLanguageChanged;
            }
        }

        private void ClearLanguageButtons()
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

        private void AddListeners()
        {

            MessageBroker.Default
                .Receive<M_SettingsShow>()
                .Subscribe(m => Show())
                .AddTo(_disposables);

            _closeButton.onClick.AddListener(OnExitButtonClicked);
            _ambientSlider.onValueChanged.AddListener(OnAmbientValueChanged);
            _sfxSlider.onValueChanged.AddListener(OnSfxValueChanged);
            _muteToggle.onValueChanged.AddListener(OnMuteValueChanged);
        }

        private void RemoveListeners()
        {
            if (_disposables != null)
                _disposables.Dispose();

            _closeButton.onClick.RemoveListener(OnExitButtonClicked);
            _ambientSlider.onValueChanged.RemoveListener(OnAmbientValueChanged);
            _sfxSlider.onValueChanged.RemoveListener(OnSfxValueChanged);
            _muteToggle.onValueChanged.RemoveListener(OnMuteValueChanged);
        }

        private void OnExitButtonClicked() => MessageBroker.Default.Publish(new M_Hide());
        private void OnLanguageChanged(string value) => _settingsModel.SetLanguage(value);
        private void OnAmbientValueChanged(float value)
        {
            _audioPlayerService.AmbientValueChanged(value);
            _settingsModel.SetAmbientVolume(value);
        }

        private void OnSfxValueChanged(float value)
        {
            _audioPlayerService.SfxValueChanged(value);
            _settingsModel.SetSfxVolume(value);
        }

        private void OnMuteValueChanged(bool value) => _settingsModel.SetMute(value);
        private void Show() => gameObject.SetActive(true);
        private void Hide() => gameObject.SetActive(false);
    }
}