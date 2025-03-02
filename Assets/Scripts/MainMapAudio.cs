using UnityEngine;

public class MainMapAudio : MonoBehaviour
{
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>() ;   
    }

    public void ButtonClick()
    {
        audioManager.PlaySFX(audioManager.menuButton);
    }
}
