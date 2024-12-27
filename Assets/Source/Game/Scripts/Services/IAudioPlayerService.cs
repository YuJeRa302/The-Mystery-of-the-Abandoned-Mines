public interface IAudioPlayerService
{
    void PlayAmbient();
    void StopAmbient();
    void PlayMainMenuAmbient();
    void StopMainMenuAmbient();
    void PlayOneShotButtonClickSound();
    void PlayOneShotButtonHoverSound();
    void PlayOneShotPopupSound();
}