using UnityEngine;

public class SettingsModel
{
    private readonly TemporaryData _temporaryData;

    public SettingsModel(TemporaryData temporaryData)
    {
        _temporaryData = temporaryData;
        AmbientVolumeValue = _temporaryData.AmbientVolume;
        SfxVolumeValue = _temporaryData.InterfaceVolume;
        IsMuted = _temporaryData.MuteStateSound;
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
    }

    public void SetSfxVolume(float volume)
    {
        SfxVolumeValue = volume;
        _temporaryData.SetInterfaceVolume(volume);
    }

    public void Mute()
    {
        IsMuted = true;
        _temporaryData.SetMuteStateSound(IsMuted);
    }

    public void UnMute()
    {
        IsMuted = false;
        _temporaryData.SetMuteStateSound(IsMuted);
    }

    public void SetMute(bool muted)
    {
        if (muted)
            Mute();
        else
            UnMute();
    }
}
