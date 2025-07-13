using Assets.Source.Game.Scripts.Services;
using UnityEngine;

namespace Assets.Source.Game.Scripts.Menu
{
    public class AudioPlayer : MonoBehaviour, IAudioPlayerService
    {
        [SerializeField] private AudioSource _ambientAudioSource;
        [SerializeField] private AudioSource _sfxAudioSource;
        [Space(20)]
        [SerializeField] private AudioClip _menuAmbientAudioClip;
        [SerializeField] private AudioClip _popupAudioClip;
        [SerializeField] private AudioClip _buttonHoverAudioClip;
        [SerializeField] private AudioClip _buttonClickAudioClip;

        public void PlayCharacterAudio(AudioClip audioClip)
        {
            _sfxAudioSource.PlayOneShot(audioClip);
        }

        public void PlayAmbient()
        {
            _ambientAudioSource.clip = _menuAmbientAudioClip;
            _ambientAudioSource.Play();
        }

        public void PlayOneShotPopupSound()
        {
            _sfxAudioSource.PlayOneShot(_popupAudioClip);
        }

        public void PlayOneShotButtonClickSound()
        {
            _sfxAudioSource.PlayOneShot(_buttonClickAudioClip);
        }

        public void PlayOneShotButtonHoverSound()
        {
            _sfxAudioSource.PlayOneShot(_buttonHoverAudioClip);
        }

        public void AmbientValueChanged(float value)
        {
            _ambientAudioSource.volume = value / 100;
        }

        public void SfxValueChanged(float value)
        {
            _sfxAudioSource.volume = value / 100;
        }

        public void StopAmbient()
        {
            _ambientAudioSource.Stop();
        }

        public void MuteSound(bool isMute)
        {
            _ambientAudioSource.mute = isMute;
            _sfxAudioSource.mute = isMute;
        }
    }
}