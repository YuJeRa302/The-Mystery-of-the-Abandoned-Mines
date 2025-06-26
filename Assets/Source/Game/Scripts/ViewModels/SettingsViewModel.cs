using System;

public class SettingsViewModel
{
    private readonly SettingsModel _settingsModel;
    private readonly MenuModel _menuModel;

    public SettingsViewModel(SettingsModel settingsModel, MenuModel menuModel)
    {
        _settingsModel = settingsModel;
        _menuModel = menuModel;
        _menuModel.InvokedSettingsShow += () => InvokedShow?.Invoke();
        _menuModel.InvokedMainMenuShow += () => InvokedHide?.Invoke();
        _menuModel.GamePaused += (state) => OnGamePause(state);
        _menuModel.GameResumed += (state) => OnGameResume(state);
    }

    public event Action InvokedShow;
    public event Action InvokedHide;

    public void Hide() => _menuModel.InvokeSettingsHide();
    public float GetAmbientVolume() => _settingsModel.AmbientVolumeValue;
    public float GetSfxVolume() => _settingsModel.SfxVolumeValue;
    public bool GetMuteStatus() => _settingsModel.IsMuted;
    public void SetLanguage(string value) => _settingsModel.SetLanguage(value);
    public void SetAmbientVolume(float volume) => _settingsModel.SetAmbientVolume(volume);
    public void SetSfxVolume(float volume) => _settingsModel.SetSfxVolume(volume);
    public void SetMuteStatus(bool value) => _settingsModel.SetMute(value);
    private void OnGamePause(bool state) => _settingsModel.OnGamePause(state);
    private void OnGameResume(bool state) => _settingsModel.OnGameResume(state);
}