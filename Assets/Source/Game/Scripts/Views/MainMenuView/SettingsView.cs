using Assets.Source.Game.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Slider _ambientSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Toggle _muteToggle;
    [SerializeField] private LanguageButtonData[] _languageButtonData;
    [SerializeField] private LanguageButtonView _languageButtonView;
    [SerializeField] private Transform _buttonsContainer;

    private List<LanguageButtonView> _languageButtonViews = new ();
    private SettingsViewModel _settingsViewModel;
    private IAudioPlayerService _audioPlayerService;

    private void OnDestroy()
    {
        ClearLanguageButton();
        RemoveListeners();
    }

    public void Initialize(SettingsViewModel settingsViewModel, IAudioPlayerService audioPlayerService)
    {
        _audioPlayerService = audioPlayerService;
        _settingsViewModel = settingsViewModel;
        _ambientSlider.value = settingsViewModel.GetAmbientVolume();
        _sfxSlider.value = settingsViewModel.GetSfxVolume();
        _muteToggle.isOn = settingsViewModel.GetMuteStatus();
        AddListeners();
        ClearLanguageButton();
        CreateLanguageButton();
        gameObject.SetActive(false);
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

    private void AddListeners()
    {
        _settingsViewModel.Showing += Show;
        _settingsViewModel.Hiding += Hide;
        _closeButton.onClick.AddListener(OnExitButtonClicked);
        _ambientSlider.onValueChanged.AddListener(OnAmbientValueChanged);
        _sfxSlider.onValueChanged.AddListener(OnSfxValueChanged);
        _muteToggle.onValueChanged.AddListener(OnMuteValueChanged);
    }

    private void RemoveListeners()
    {
        _settingsViewModel.Showing -= Show;
        _settingsViewModel.Hiding -= Hide;
        _closeButton.onClick.RemoveListener(OnExitButtonClicked);
        _ambientSlider.onValueChanged.RemoveListener(OnAmbientValueChanged);
        _sfxSlider.onValueChanged.RemoveListener(OnSfxValueChanged);
        _muteToggle.onValueChanged.RemoveListener(OnMuteValueChanged);
    }

    private void OnExitButtonClicked() => _settingsViewModel.Hide();
    private void OnLanguageChanged(string value) => _settingsViewModel.SetLanguage(value);
    private void OnAmbientValueChanged(float value)
    {
        _audioPlayerService.AmbientValueChanged(value);
        _settingsViewModel.SetAmbientVolume(value);
    }

    private void OnSfxValueChanged(float value)
    {
        _audioPlayerService.SfxValueChanged(value);
        _settingsViewModel.SetSfxVolume(value);
    }

    private void OnMuteValueChanged(bool value) => _settingsViewModel.SetMuteStatus(value);
    private void Show() => gameObject.SetActive(true);
    private void Hide() => gameObject.SetActive(false);
}