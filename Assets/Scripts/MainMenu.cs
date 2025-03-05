using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        if (audioManager != null)
        {
            audioManager.PlayMusic(audioManager.menuBackGround);
        }
    }

    public void PlayGame()
    {
        audioManager.PlaySFX(audioManager.menuButton);
        Invoke("LoadGame", 0.5f);
    }

    private void LoadGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void ExitGame()
    {
        audioManager.PlaySFX(audioManager.menuButton);
        Application.Quit();
    }

    public void Continue()
    {
        audioManager.PlaySFX(audioManager.menuButton);
    }

    public void MenuSetting()
    {
        audioManager.PlaySFX(audioManager.menuButton);
    }

    public void MenuCancel()
    {
        audioManager.PlaySFX(audioManager.menuButton);
    }

    public void Tutorial()
    {
        audioManager.PlaySFX(audioManager.menuButton);
    }


}
