using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSpriteChanger : MonoBehaviour
{
    AudioManager audioManager;
    public Sprite newSprite; // Sprite mới khi đủ 5 lần hoàn thành
    public GameObject Player; // Player cần ẩn

    private SpriteRenderer spriteRenderer;
    private bool hasChangedSprite = false; // Đảm bảo chỉ đổi sprite một lần

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!hasChangedSprite && CarRepairMiniGame.MinigameCompletionCount >= 5)
        {
            ChangeCarSprite();
        }
    }

    private void ChangeCarSprite()
    {
        if (spriteRenderer != null && newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
            Debug.Log("Đã đổi sprite của xe!");

            hasChangedSprite = true; // Đánh dấu đã đổi sprite

            // Gọi Coroutine chờ 5 giây rồi ẩn player và chuyển scene
            StartCoroutine(HidePlayerAndChangeScene());
        }
    }

    IEnumerator HidePlayerAndChangeScene()
    {
        audioManager.StopMusic();
        audioManager.StopSFX();
        yield return new WaitForSeconds(5f); // Chờ 5 giây

        if (Player != null)
        {
            Player.SetActive(false); // Ẩn Player
        }

        SceneManager.LoadScene(4); // Chuyển Scene 4
    }
}
