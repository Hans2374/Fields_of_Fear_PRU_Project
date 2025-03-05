using UnityEngine;

public class MainMapAudio : MonoBehaviour
{
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>() ;   
    }

    private void Start()
    {
        if (audioManager != null)
        {
            audioManager.PlayMusic(audioManager.morningSound);
            audioManager.PlaySFX(audioManager.birdSound);
        }
    }

    public void ButtonClick()
    {
        audioManager.PlaySFX(audioManager.menuButton);
    }
}
