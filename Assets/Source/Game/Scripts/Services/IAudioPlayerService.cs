namespace Assets.Source.Game.Scripts.Services
{
    public interface IAudioPlayerService
    {
        void PlayAmbient();
        void StopAmbient();
        void PlayOneShotButtonClickSound();
        void PlayOneShotButtonHoverSound();
        void PlayOneShotPopupSound();
        public void AmbientValueChanged(float value);
        public void SfxValueChanged(float value);
        public void MuteSound(bool isMute);
    }
}