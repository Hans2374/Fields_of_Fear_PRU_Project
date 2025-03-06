using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Import SceneManager

public class CutScene : MonoBehaviour
{
    AudioManager audioManager;
    public Image carImage; // UI Image của xe
    public Sprite brokenCarSprite; // Sprite xe hỏng
    public BackgroundLooper[] backgroundLoopers; // Mảng chứa BG1 & BG2
    public float timeBeforeCarBreaks = 10f;
    public float carMoveSpeed = 600f; // Tốc độ xe chạy
    public AudioClip explosionSound; // Âm thanh nổ
    public float timeBeforeMove = 3f; // Thời gian chờ trước khi chuyển scene

    private RectTransform carRect;
    private bool isCarMoving = false;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        carRect = carImage.GetComponent<RectTransform>();
    }

    private void Start()
    {
        audioManager.PlayMusic(audioManager.menuBackGround);
        StartCoroutine(CarBreaksDown());
    }

    IEnumerator CarBreaksDown()
    {
        yield return new WaitForSeconds(timeBeforeCarBreaks);

        // Dừng background loop
        foreach (BackgroundLooper bg in backgroundLoopers)
        {
            bg.StopLoop();
        }

        // Đổi ảnh xe thành xe hỏng
        carImage.sprite = brokenCarSprite;

        // Bắt đầu di chuyển xe
        isCarMoving = true;
    }

    private void Update()
    {
        if (isCarMoving)
        {
            // Di chuyển xe từ trái sang phải
            carRect.anchoredPosition += Vector2.right * carMoveSpeed * Time.deltaTime;

            // Khi xe ra khỏi màn hình
            if (carRect.anchoredPosition.x >= Screen.width)
            {
                isCarMoving = false;
                StartCoroutine(CarExplosion());
            }
        }
    }

    IEnumerator CarExplosion()
    {
        audioManager.StopMusic(); // Dừng nhạc nền
        audioManager.PlaySFX(audioManager.carMove); // Phát âm thanh xe di chuyển
        yield return new WaitForSeconds(timeBeforeMove); // Chờ một chút
        SceneManager.LoadSceneAsync(1); // Chuyển scene
    }
}
