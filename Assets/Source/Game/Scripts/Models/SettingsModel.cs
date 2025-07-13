using Assets.Source.Game.Scripts.Menu;
using Assets.Source.Game.Scripts.Services;
using Lean.Localization;

namespace Assets.Source.Game.Scripts.Models
{
    public class SettingsModel
    {
        private readonly LeanLocalization _leanLocalization;
        private readonly AudioPlayer _audioPlayer;
        private readonly PersistentDataService _persistentDataService;

        public SettingsModel(PersistentDataService persistentDataService, LeanLocalization leanLocalization, AudioPlayer audioPlayer)
        {
            _leanLocalization = leanLocalization;
            _persistentDataService = persistentDataService;
            _audioPlayer = audioPlayer;
            AmbientVolumeValue = _persistentDataService.PlayerProgress.AmbientVolume;
            SfxVolumeValue = _persistentDataService.PlayerProgress.SfxVolume;
            IsMuted = _persistentDataService.PlayerProgress.IsMuted;
            _audioPlayer.AmbientValueChanged(AmbientVolumeValue);
            _audioPlayer.SfxValueChanged(SfxVolumeValue);
            _audioPlayer.PlayAmbient();
            _audioPlayer.MuteSound(IsMuted);
            SetLanguage(_persistentDataService.PlayerProgress.Language);
        }

        public float AmbientVolumeValue { get; private set; }
        public float SfxVolumeValue { get; private set; }
        public bool IsMuted { get; private set; }

        public void SetAmbientVolume(float volume)
        {
            AmbientVolumeValue = volume;
            _persistentDataService.PlayerProgress.AmbientVolume = volume;
        }

        public void SetLanguage(string value)
        {
            _persistentDataService.PlayerProgress.Language = value;
            _leanLocalization.SetCurrentLanguage(value);
        }

        public void SetSfxVolume(float volume)
        {
            SfxVolumeValue = volume;
            _persistentDataService.PlayerProgress.SfxVolume = volume;
        }

        public void OnGamePause(bool state)
        {
            if (_audioPlayer != null)
                _audioPlayer.MuteSound(!state);
        }

        public void OnGameResume(bool state)
        {
            if (_audioPlayer != null)
                _audioPlayer.MuteSound(_persistentDataService.PlayerProgress.IsMuted);
        }

        public void Mute()
        {
            IsMuted = true;
            _persistentDataService.PlayerProgress.IsMuted = IsMuted;

            if (_audioPlayer != null)
                _audioPlayer.MuteSound(IsMuted);
        }

        public void UnMute()
        {
            IsMuted = false;
            _persistentDataService.PlayerProgress.IsMuted = IsMuted;
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
}