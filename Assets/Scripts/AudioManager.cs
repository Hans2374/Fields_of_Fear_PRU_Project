using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---Audio Source---")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("---Audio Clip---")]
    public AudioClip menuBackGround;
    public AudioClip menuButton;

    public AudioClip moveStep;
    public AudioClip getHit;
    public AudioClip monsterRoar;
    public AudioClip monsterChase;

    private void Start()
    {
        musicSource.clip = menuBackGround;
        musicSource.Play();   
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void StopSFX()
{
    SFXSource.Stop();
}
}
