using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---Audio Source---")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("---Audio Clip---")]
    public AudioClip menuBackGround;
    public AudioClip morningSound;
    public AudioClip nightSound;
    public AudioClip menuButton;
    public AudioClip moveStep;
    public AudioClip getHit;
    public AudioClip monsterRoar;
    public AudioClip monsterChase;
    public AudioClip water;
    public AudioClip birdSound;
    public AudioClip carMove;
    public AudioClip crops;
    public AudioClip carFail;
    public AudioClip carIncrease;
    public AudioClip carDone;


     private void Start()
    {
        musicSource.clip = menuBackGround;
        musicSource.Play();   
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }
    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void StopSFX()
    {
        SFXSource.Stop();
    }

    public bool IsMusicPlaying(AudioClip clip)
    {
        return musicSource.isPlaying && musicSource.clip == clip;
    }
    public bool IsSFXPlaying(AudioClip clip)
    {
        return SFXSource.isPlaying && SFXSource.clip == clip;
    }
}
