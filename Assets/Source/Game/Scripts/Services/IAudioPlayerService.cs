namespace Assets.Source.Game.Scripts.Services
{
    public interface IAudioPlayerService
    {
        public void PlayOneShotButtonClickSound();
        public void PlayOneShotButtonHoverSound();
        public void PlayOneShotPopupSound();
        public void AmbientValueChanged(float value);
        public void SfxValueChanged(float value);
    }
}