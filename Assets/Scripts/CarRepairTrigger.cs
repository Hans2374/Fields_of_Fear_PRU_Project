using UnityEngine;

public class CarRepairTrigger : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("UI mini-game sẽ bật lên khi kích hoạt sửa xe")]
    public GameObject miniGameUI;

    [Tooltip("Thông báo cho Player: 'Nhấn E để sửa xe'")]
    public GameObject repairPromptUI;

    // Biến kiểm tra Player có trong vùng trigger không
    private bool isPlayerInside = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng đi vào có tag 'Player' không
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            // Hiển thị thông báo nếu có
            if (repairPromptUI != null)
            {
                repairPromptUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Khi Player rời vùng trigger
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            // Ẩn thông báo nếu có
            if (repairPromptUI != null)
            {
                repairPromptUI.SetActive(false);
            }
        }
    }

    private void Update()
    {
        // Nếu Player ở trong vùng và nhấn phím E
        if (isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            // Kích hoạt UI mini-game
            if (miniGameUI != null)
            {
                miniGameUI.SetActive(true);
            }

            // Ẩn thông báo sau khi kích hoạt mini-game
            if (repairPromptUI != null)
            {
                repairPromptUI.SetActive(false);
            }

            // Tùy chọn: Khóa di chuyển của Player
            // Giả sử bạn có một script PlayerMovement gắn trên Player

            CharacterMovement playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
            

            // Tùy chọn: Ẩn các thành phần HUD khác nếu cần
            // Ví dụ: HUDManager.Instance.SetActive(false);
        }
    }
}
