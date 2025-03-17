using UnityEngine;
using UnityEngine.UI;

public class CarRepairMiniGame : MonoBehaviour
{
    AudioManager audioManager;
    [Header("Thanh trượt chạy qua lại")]
    public Slider movingSlider;      // Slider "điểm" di chuyển
    public float moveSpeed = 1f;     // Tốc độ di chuyển
    private bool movingRight = true; // Hướng di chuyển
    private float sliderValue = 0f;

    [Header("Vùng 'đẹp' để nhấn Space (0..1)")]
    public float goodZoneMin = 0.45f;
    public float goodZoneMax = 0.55f;

    [Header("Thanh tiến độ sửa xe")]
    public Slider progressSlider;    // Slider cho tiến độ
    public float successIncrement = 0.2f; // Mỗi lần bấm đúng, tăng 20%

    [Header("UI Thông báo")]
    public GameObject insufficientPartsMessage; // Thông báo khi không đủ bộ phận xe

    public static int MinigameCompletionCount { get; private set; } = 0;

    private bool hasVerifiedParts = false;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
    }

    private void OnEnable()
    {
        // Khi miniGameUI được bật, reset trạng thái
        sliderValue = 0f;
        movingRight = true;
        movingSlider.value = 0f;
        progressSlider.value = 0f;

        // Kiểm tra lại khi mini-game được bật để đảm bảo an toàn
        hasVerifiedParts = CheckForCarParts();

        // Nếu không có car part, tắt mini-game
        if (!hasVerifiedParts)
        {
            ShowInsufficientPartsMessage();
            Invoke("EndMinigame", 2f);
        }
    }

    void Update()
    {
        // Nếu không có bộ phận xe, dừng xử lý
        if (!hasVerifiedParts) return;

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
            // Kiểm tra xem sliderValue đang nằm trong vùng "đẹp"
            if (sliderValue >= goodZoneMin && sliderValue <= goodZoneMax)
            {
                // Thành công
                progressSlider.value += successIncrement;
                audioManager.PlaySFX(audioManager.carIncrease);

                // Kiểm tra nếu progress đầy
                if (progressSlider.value >= 1f)
                {
                    // Kiểm tra lại một lần nữa để đảm bảo an toàn
                    if (CheckForCarParts())
                    {
                        Debug.Log("Sửa xe xong!");
                        MinigameCompletionCount++; // Tăng số lần hoàn thành
                        Debug.Log("Số lần hoàn thành mini-game: " + MinigameCompletionCount);

                        // Remove car part from inventory
                        RemoveCarPartFromInventory();
                    }
                    else
                    {
                        Debug.LogError("Không thể hoàn thành sửa xe - không có bộ phận xe trong túi đồ!");
                        ShowInsufficientPartsMessage();
                    }

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

    // Kiểm tra xem có bộ phận xe trong túi đồ không
    private bool CheckForCarParts()
    {
        // Get player's inventory
        CharacterMovement player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<CharacterMovement>();
        if (player == null)
        {
            Debug.LogError("Player not found when checking for car parts!");
            return false;
        }

        // Get inventory via GetInventory method
        Inventory inventory = null;
        var methodInfo = player.GetType().GetMethod("GetInventory");
        if (methodInfo != null)
        {
            inventory = methodInfo.Invoke(player, null) as Inventory;
        }

        // Fallback - try to get inventory from InventoryUI
        if (inventory == null)
        {
            InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
            if (inventoryUI != null && inventoryUI.inventory != null)
            {
                inventory = inventoryUI.inventory;
            }
        }

        if (inventory != null)
        {
            // Check if there's a car part
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

    private void RemoveCarPartFromInventory()
    {
        // Get player's inventory
        CharacterMovement player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<CharacterMovement>();
        if (player == null)
        {
            Debug.LogError("Player not found when trying to remove car part!");
            return;
        }

        // Get inventory via GetInventory method
        Inventory inventory = null;
        var methodInfo = player.GetType().GetMethod("GetInventory");
        if (methodInfo != null)
        {
            inventory = methodInfo.Invoke(player, null) as Inventory;
        }

        // Fallback - try to get inventory from InventoryUI
        if (inventory == null)
        {
            InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
            if (inventoryUI != null && inventoryUI.inventory != null)
            {
                inventory = inventoryUI.inventory;
            }
        }

        if (inventory != null)
        {
            // Find a car part in inventory
            foreach (Item item in inventory.GetItems())
            {
                if (item.itemType == Item.ItemType.CarPart && item.amount > 0)
                {
                    // Create a new item to remove (to avoid modifying during iteration)
                    Item carPartToRemove = new Item
                    {
                        itemType = Item.ItemType.CarPart,
                        amount = 1
                    };

                    // Remove it from inventory
                    inventory.RemoveItem(carPartToRemove);
                    Debug.Log("Car part removed from inventory after successful repair!");

                    // Update UI
                    InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
                    if (inventoryUI != null)
                    {
                        inventoryUI.RefreshInventoryItems();
                    }

                    break;
                }
            }
        }
        else
        {
            Debug.LogError("Failed to find inventory to remove car part!");
        }
    }

    private void ShowInsufficientPartsMessage()
    {
        if (insufficientPartsMessage != null)
        {
            insufficientPartsMessage.SetActive(true);
            Invoke("HideInsufficientPartsMessage", 2f);
        }
        else
        {
            Debug.LogWarning("Missing insufficientPartsMessage reference in CarRepairMiniGame!");
        }
    }

    private void HideInsufficientPartsMessage()
    {
        if (insufficientPartsMessage != null)
        {
            insufficientPartsMessage.SetActive(false);
        }
    }

    private void EndMinigame()
    {
        // Tắt UI
        gameObject.SetActive(false);

        CharacterMovement playerMovement = GameObject.FindGameObjectWithTag("Player")?.GetComponent<CharacterMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
        // (Tuỳ ý) Mở lại di chuyển Player, ẩn/hiện HUD, v.v.
    }
}