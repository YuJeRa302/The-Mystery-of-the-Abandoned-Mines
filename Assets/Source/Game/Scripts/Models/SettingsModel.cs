using UnityEngine;
using Lean.Localization;

public class SettingsModel
{
    private readonly TemporaryData _temporaryData;
    private readonly LeanLocalization _leanLocalization;
    private readonly AudioPlayer _audioPlayer;

    public SettingsModel(TemporaryData temporaryData, LeanLocalization leanLocalization, AudioPlayer audioPlayer)
    {
        _temporaryData = temporaryData;
        _leanLocalization = leanLocalization;
        _audioPlayer = audioPlayer;
        AmbientVolumeValue = _temporaryData.AmbientVolume;
        SfxVolumeValue = _temporaryData.InterfaceVolume;
        IsMuted = _temporaryData.MuteStateSound;
        _audioPlayer.AmbientValueChanged(AmbientVolumeValue);
        _audioPlayer.SfxValueChanged(SfxVolumeValue);
        _audioPlayer.PlayMainMenuAmbient();
        _audioPlayer.MuteSound(IsMuted);
        SetLanguage(_temporaryData.Language);
    }

    public string LanguageTag { get; private set; }
    public float AmbientVolumeValue { get; private set; }
    public float SfxVolumeValue { get; private set; }
    public bool IsMuted { get; private set; }

    public void SetAmbientVolume(float volume)
    {
        AmbientVolumeValue = volume;
        _temporaryData.SetAmbientVolume(volume);
    }

    public void SetLanguage(string value)
    {
        LanguageTag = value;
        _temporaryData.SetCurrentLanguage(value);
        _leanLocalization.SetCurrentLanguage(value);
    }

    public void SetSfxVolume(float volume)
    {
        SfxVolumeValue = volume;
        _temporaryData.SetInterfaceVolume(volume);
    }

    public void OnGamePause(bool state) 
    {
        _audioPlayer.MuteSound(!state);
    }

    public void OnGameResume(bool state) 
    {
        _audioPlayer.MuteSound(_temporaryData.MuteStateSound);
    }

    public void Mute()
    {
        IsMuted = true;
        _temporaryData.SetMuteStateSound(IsMuted);
        _audioPlayer.MuteSound(IsMuted);
    }

    public void UnMute()
    {
        IsMuted = false;
        _temporaryData.SetMuteStateSound(IsMuted);
        _audioPlayer.MuteSound(IsMuted);
    }

    public void SetMute(bool muted)
    {
        if (muted)
            Mute();
        else
            UnMute();
    }
}