using UnityEngine;
using UnityEngine.UI;

public class CarRepairMiniGame : MonoBehaviour
{
    AudioManager audioManager;
    [Header("Thanh trượt chạy qua lại")]
    public Slider movingSlider;      // Slider “điểm” di chuyển
    public float moveSpeed = 1f;     // Tốc độ di chuyển
    private bool movingRight = true; // Hướng di chuyển
    private float sliderValue = 0f;

    [Header("Vùng 'đẹp' để nhấn Space (0..1)")]
    public float goodZoneMin = 0.45f;
    public float goodZoneMax = 0.55f;

    [Header("Thanh tiến độ sửa xe")]
    public Slider progressSlider;    // Slider cho tiến độ
    public float successIncrement = 0.2f; // Mỗi lần bấm đúng, tăng 20%

    public static int MinigameCompletionCount { get; private set; } = 0;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    private void OnEnable()
    {
        // Khi miniGameUI được bật, reset trạng thái
        sliderValue = 0f;
        movingRight = true;
        movingSlider.value = 0f;

        progressSlider.value = 0f; // hoặc giữ nguyên nếu muốn
    }

    void Update()
    {
        // 1) Di chuyển slider qua lại
        float direction = movingRight ? 1f : -1f;
        sliderValue += direction * moveSpeed * Time.deltaTime;

        if (sliderValue >= 1f)
        {
            sliderValue = 1f;
            movingRight = false;
        }
        else if (sliderValue <= 0f)
        {
            sliderValue = 0f;
            movingRight = true;
        }

        movingSlider.value = sliderValue;

        // 2) Người chơi nhấn Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Kiểm tra xem sliderValue đang nằm trong vùng “đẹp”
            if (sliderValue >= goodZoneMin && sliderValue <= goodZoneMax)
            {
                // Thành công
                progressSlider.value += successIncrement;
                audioManager.PlaySFX(audioManager.carIncrease);

                // Kiểm tra nếu progress đầy
                if (progressSlider.value >= 1f)
                {
                    Debug.Log("Sửa xe xong!");
                    MinigameCompletionCount++; // Tăng số lần hoàn thành
                    Debug.Log("Số lần hoàn thành mini-game: " + MinigameCompletionCount);
                    EndMinigame();
                }
            }
            else
            {
                // Thất bại hoặc phạt
                Debug.Log("Nhấn sai thời điểm!");
                audioManager.PlaySFX(audioManager.carFail);
            }
        }

        // 3) Thoát mini-game nếu muốn (vd nhấn Esc)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndMinigame();
        }
    }

    private void EndMinigame()
    {
        // Tắt UI
        gameObject.SetActive(false);

        CharacterMovement playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
        // (Tuỳ ý) Mở lại di chuyển Player, ẩn/hiện HUD, v.v.
    }
}


