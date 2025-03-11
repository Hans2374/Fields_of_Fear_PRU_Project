using UnityEngine;

public class FarmLandManager : MonoBehaviour
{
    public CharacterMovement player;
    public GameObject farmLandPrefab;  // Prefab của ô đất
    public int columns = 3;  // Số cột
    public int rows = 9;  // Số hàng
    public float spacingX = 1.5f;  // Khoảng cách giữa các ô theo trục X
    public float spacingY = 1.5f;  // Khoảng cách giữa các ô theo trục Y
    public Vector2 startPosition = new Vector2(0, 0);  // Vị trí ô đất đầu tiên
    public InventoryUI inventoryUI;  // Kết nối với UI kho đồ
    private void Start()
    {
        GenerateFarmLands();
        if (player == null)
    {
        player = FindObjectOfType<CharacterMovement>();
    }
    }

    private void GenerateFarmLands()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector2 position = new Vector2(
                    startPosition.x + col * spacingX,
                    startPosition.y - row * spacingY  // Trừ đi để ô đất xếp xuống dưới
                );

                Instantiate(farmLandPrefab, position, Quaternion.identity, transform);
                Debug.Log($"🟫 Tạo ô đất tại {position}");
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Click chuột trái để trồng cây
        {
            if (player != null && !player.isWatering)
        {
            Debug.Log("⚠ Bạn phải tưới nước trước khi trồng cây!");
            return;
        }
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log($"🖱 Click vào: {hit.collider.gameObject.name}, Tag: {hit.collider.tag}");

                if (hit.collider.CompareTag("Farmland"))
                {
                    Farmland farmland = hit.collider.GetComponent<Farmland>();

                    if (farmland != null)
                    {
                        Debug.Log("✅ Đã nhận diện ô Farmland!");

                        if (inventoryUI.selectedItem != null)
                        {
                            Debug.Log($"🌱 Đang thử trồng cây từ item: {inventoryUI.selectedItem.itemType}");

                            if (inventoryUI.selectedItem.crop != null)
                            {
                                // ✅ Kiểm tra nếu hết hạt giống thì không cho trồng
                                if (inventoryUI.selectedItem.amount <= 0)
                                {
                                    Debug.Log("⚠ Không còn hạt giống, không thể trồng cây!");
                                    return;
                                }

                                Debug.Log($"🌱 Dữ liệu cây trồng hợp lệ: {inventoryUI.selectedItem.crop.cropName}");
                                farmland.PlantCrop(inventoryUI.selectedItem.crop);

                                // 🔻 Trừ số lượng hạt giống
                                inventoryUI.selectedItem.amount--;

                                // Nếu hết hạt giống, xóa khỏi kho đồ
                                if (inventoryUI.selectedItem.amount <= 0)
                                {
                                    inventoryUI.inventory.RemoveItem(inventoryUI.selectedItem);
                                    inventoryUI.selectedItem = null; // Xóa item đang chọn
                                }

                                // Cập nhật lại UI kho đồ
                                inventoryUI.UpdateInventoryUI();
                            }
                            else
                            {
                                Debug.Log("⚠ Item đã chọn không có dữ liệu cây trồng!");
                            }
                        }
                        else
                        {
                            Debug.Log("⚠ Chưa chọn hạt giống!");
                        }
                    }
                    else
                    {
                        Debug.Log("⚠ Không tìm thấy component Farmland!");
                    }
                }
                else
                {
                    Debug.Log("⚠ Click không phải vào Farmland!");
                }
            }
            else
            {
                Debug.Log("⚠ Raycast không trúng gì cả!");
            }
        }

        //if (Input.GetMouseButtonDown(0)) // Click chuột trái để trồng cây
        //{
        //    Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        //    if (hit.collider != null)
        //    {
        //        Debug.Log($"🖱 Click vào: {hit.collider.gameObject.name}, Tag: {hit.collider.tag}");

        //        if (hit.collider.CompareTag("Farmland"))
        //        {
        //            Farmland farmland = hit.collider.GetComponent<Farmland>();

        //            if (farmland != null)
        //            {
        //                Debug.Log("✅ Đã nhận diện ô Farmland!");

        //                if (inventoryUI.selectedItem != null)
        //                {
        //                    Debug.Log($"🌱 Đang thử trồng cây từ item: {inventoryUI.selectedItem.itemType}");

        //                    if (inventoryUI.selectedItem.crop != null)
        //                    {
        //                        Debug.Log($"🌱 Dữ liệu cây trồng hợp lệ: {inventoryUI.selectedItem.crop.cropName}");
        //                        farmland.PlantCrop(inventoryUI.selectedItem.crop);

        //                        // Trừ số lượng hạt giống trong kho
        //                        inventoryUI.selectedItem.amount--;
        //                        if (inventoryUI.selectedItem.amount <= 0)
        //                        {
        //                            inventoryUI.inventory.RemoveItem(inventoryUI.selectedItem);
        //                        }

        //                        // Cập nhật lại UI kho đồ
        //                        inventoryUI.UpdateInventoryUI();
        //                    }
        //                    else
        //                    {
        //                        Debug.Log("⚠ Item đã chọn không có dữ liệu cây trồng!");
        //                    }
        //                }
        //                else
        //                {
        //                    Debug.Log("⚠ Chưa chọn hạt giống!");
        //                }
        //            }
        //            else
        //            {
        //                Debug.Log("⚠ Không tìm thấy component Farmland!");
        //            }
        //        }
        //        else
        //        {
        //            Debug.Log("⚠ Click không phải vào Farmland!");
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("⚠ Raycast không trúng gì cả!");
        //    }
        //}
    }

}
