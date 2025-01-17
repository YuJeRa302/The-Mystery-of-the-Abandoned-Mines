using UnityEngine;

public class AudioPlayer : MonoBehaviour, IAudioPlayerService
{
    [SerializeField] private AudioSource _ambientAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;
    [Space(20)]
    [SerializeField] private AudioClip _menuAmbientAudioClip;
    [SerializeField] private AudioClip _sfxAudioClip;
    [SerializeField] private AudioClip _popupAudioClip;
    [SerializeField] private AudioClip _buttonHoverAudioClip;
    [SerializeField] private AudioClip _buttonClickAudioClip;

    public void PlayAmbient()
    {
        throw new System.NotImplementedException();
    }

    public void PlayMainMenuAmbient()
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

    public void PlayOneShotButtonSound()
    {
        throw new System.NotImplementedException();
    }

    public void StopAmbient()
    {
        throw new System.NotImplementedException();
    }

    public void StopMainMenuAmbient()
    {
        _ambientAudioSource.Stop();
    }
}
