public interface IAudioPlayerService
{
    void PlayMainMenuAmbient();
    void StopMainMenuAmbient();
    void PlayOneShotButtonClickSound();
    void PlayOneShotButtonHoverSound();
    void PlayOneShotPopupSound();
    public void AmbientValueChanged(float value);
    public void SfxValueChanged(float value);
    public void MuteSound(bool isMute);
}