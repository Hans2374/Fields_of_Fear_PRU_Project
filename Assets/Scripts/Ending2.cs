using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Ending2 : MonoBehaviour
{
    AudioManager audioManager;

    public Image carImage; // UI Image của xe
    public Sprite brokenCarSprite; // Sprite xe hỏng
    public BackgroundLooper[] backgroundLoopers; // Mảng chứa BG1 & BG2

    public float timeBeforeCarBreaks = 10f;
    public float carMoveSpeed = 600f;
    public float timeBeforeMove = 3f;

    private RectTransform carRect;
    private bool isCarMoving = false;

    // Thêm 3 UI Image cho chữ "Thanks", "For", "Playing"
    public Image thanksImage;
    public Image forImage;
    public Image playingImage;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        carRect = carImage.GetComponent<RectTransform>();

        // Ẩn tất cả chữ ban đầu
        thanksImage.gameObject.SetActive(false);
        forImage.gameObject.SetActive(false);
        playingImage.gameObject.SetActive(false);
    }

    private void Start()
    {
        audioManager.PlayMusic(audioManager.menuBackGround);
        StartCoroutine(CarBreaksDown());
    }

    IEnumerator CarBreaksDown()
    {
        yield return new WaitForSeconds(timeBeforeCarBreaks);

        foreach (BackgroundLooper bg in backgroundLoopers)
        {
            bg.StopLoop();
        }

        carImage.sprite = brokenCarSprite;
        isCarMoving = true;
    }

    private void Update()
    {
        if (isCarMoving)
        {
            carRect.anchoredPosition += Vector2.left * carMoveSpeed * Time.deltaTime;

            if (carRect.anchoredPosition.x <= -Screen.width / 2 - carRect.rect.width)
            {
                isCarMoving = false;
                StartCoroutine(CarExplosion());
            }
        }
    }

    IEnumerator CarExplosion()
    {
        audioManager.StopMusic();
        audioManager.StopSFX();
        yield return new WaitForSeconds(timeBeforeMove);

        yield return new WaitForSeconds(3f); // Chờ 3 giây trước khi hiển thị chữ
        audioManager.PlayMusic(audioManager.morningSound);

        // Hiển thị từng chữ một, mỗi chữ cách nhau 1 giây
        thanksImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        forImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        playingImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);

    }


}
