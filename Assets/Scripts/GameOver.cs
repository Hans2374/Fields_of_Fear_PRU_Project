using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        audioManager.StopSFX();
        audioManager.StopMusic();
        if (audioManager != null)
        {
            audioManager.PlayMusic(audioManager.menuBackGround);
        }
    }
    public void ExitGame()
    {
        audioManager.PlaySFX(audioManager.menuButton);
        Application.Quit();
    }

    public void MainMenu()
    {
        audioManager.PlaySFX(audioManager.menuButton);
        Invoke("LoadGame", 0.5f);
        audioManager.StopMusic();
    }

    private void LoadGame()
    {
        SceneManager.LoadSceneAsync(1);
    }
}
