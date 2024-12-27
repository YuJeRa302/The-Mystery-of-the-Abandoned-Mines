using UnityEngine;

public class AudioPlayer : MonoBehaviour, IAudioPlayerService
{
    [SerializeField] private AudioSource _ambientAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;
    [Space(20)]
    [SerializeField] private AudioClip _menuAmbientAudioClip;
    [SerializeField] private AudioClip _sfxAudioClip;
    [SerializeField] private AudioClip _popupAudioClip;

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
        throw new System.NotImplementedException();
    }

    public void PlayOneShotButtonHoverSound()
    {
        throw new System.NotImplementedException();
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
