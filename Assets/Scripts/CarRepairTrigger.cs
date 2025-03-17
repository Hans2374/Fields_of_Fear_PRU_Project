using UnityEngine;

public class CarRepairTrigger : MonoBehaviour
{
    // Reference to audio manager to play sounds
    private AudioManager audioManager;

    [Header("UI Settings")]
    [Tooltip("UI mini-game sẽ bật lên khi kích hoạt sửa xe")]
    public GameObject miniGameUI;

    [Tooltip("Thông báo cho Player: 'Nhấn E để sửa xe'")]
    public GameObject repairPromptUI;

    // Biến kiểm tra Player có trong vùng trigger không
    private bool isPlayerInside = false;

    private void Awake()
    {
        // Find audio manager if not already found
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng đi vào có tag 'Player' không
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            UpdatePrompts();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Khi Player rời vùng trigger
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;

            // Ẩn tất cả thông báo
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
            // Kiểm tra xem Player có bộ phận xe trong túi đồ không
            if (HasCarPartInInventory())
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

                // Khóa di chuyển của Player
                CharacterMovement playerMovement = GameObject.FindGameObjectWithTag("Player")?.GetComponent<CharacterMovement>();
                if (playerMovement != null)
                {
                    playerMovement.enabled = false;
                }
            }
            else
            {
                // Hiển thị thông báo "E" prompt (cũng hiển thị khi không có bộ phận xe)
                // Giữ nguyên prompt này vì nó là popup hiện có trong game
                if (repairPromptUI != null)
                {
                    repairPromptUI.SetActive(true);
                }

                // Phát âm thanh thất bại
                if (audioManager != null)
                {
                    audioManager.PlaySFX(audioManager.carFail);
                }

                Debug.Log("Không có bộ phận xe trong túi đồ!");
            }
        }
    }

    // Kiểm tra xem Player có bộ phận xe trong túi đồ không
    private bool HasCarPartInInventory()
    {
        // Tìm Player và lấy Inventory
        CharacterMovement player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<CharacterMovement>();
        if (player == null) return false;

        Inventory inventory = null;

        // Cách 1: Qua phương thức GetInventory
        var methodInfo = player.GetType().GetMethod("GetInventory");
        if (methodInfo != null)
        {
            inventory = methodInfo.Invoke(player, null) as Inventory;
        }

        // Cách 2: Qua InventoryUI
        if (inventory == null)
        {
            InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
            if (inventoryUI != null && inventoryUI.inventory != null)
            {
                inventory = inventoryUI.inventory;
            }
        }

        // Kiểm tra có car part không
        if (inventory != null)
        {
            foreach (Item item in inventory.GetItems())
            {
                if (item.itemType == Item.ItemType.CarPart && item.amount > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Cập nhật hiển thị thông báo dựa trên tình trạng túi đồ
    private void UpdatePrompts()
    {
        // Luôn hiển thị prompt "E" như trước đây
        if (repairPromptUI != null)
        {
            repairPromptUI.SetActive(true);
        }
    }
}