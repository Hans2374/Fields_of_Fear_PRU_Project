using UnityEngine;

public class Audio : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioSource vfxAudioSource;
    public AudioClip musicClip;


    void Start()
    {
        musicAudioSource.clip = musicClip;
        musicAudioSource.Play();
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        vfxAudioSource.clip = sfxClip;
        vfxAudioSource.PlayOneShot(sfxClip);
    }
    
}
