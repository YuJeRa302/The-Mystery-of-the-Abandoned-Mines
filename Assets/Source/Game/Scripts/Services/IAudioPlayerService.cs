public interface IAudioPlayerService
{
    void PlayAmbient();
    void StopAmbient();
    void PlayMainMenuAmbient();
    void StopMainMenuAmbient();
    void PlayOneShotButtonClickSound();
    void PlayOneShotButtonHoverSound();
    void PlayOneShotPopupSound();
    public void AmbientValueChanged(float value);
    public void SfxValueChanged(float value);
    public void MuteSoundPayse(bool isMute);
    public void MuteSoundSettings(bool isMute);
}