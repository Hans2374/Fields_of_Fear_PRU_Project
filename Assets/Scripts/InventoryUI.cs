using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject itemSlotPrefab; // Prefab của ItemSlot
    public Transform slotParent; // Panel chứa các slot

    private Dictionary<ItemType, GameObject> itemSlots = new Dictionary<ItemType, GameObject>();

    void Start()
    {
        if (itemSlotPrefab == null)
        {
            Debug.LogError("❌ itemSlotPrefab chưa được gán! Kiểm tra trong Inspector hoặc đường dẫn Resources.");
        }

        Inventory.Instance.OnInventoryChanged += UpdateUI;
        UpdateUI();
    }


    void UpdateUI()
    {
        if (slotParent == null)
        {
            Debug.LogError("❌ slotParent chưa được gán trong Inspector!");
            return;
        }

        // Xóa UI cũ trước khi cập nhật mới
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        Dictionary<ItemType, int> items = Inventory.Instance.GetItems();
        foreach (var item in items)
        {
            GameObject slot = Instantiate(itemSlotPrefab, slotParent);
            ItemSlotUI slotUI = slot.GetComponent<ItemSlotUI>();
            if (slotUI == null)
            {
                Debug.LogError("❌ Prefab itemSlotPrefab không có script ItemSlotUI!");
                return;
            }
            slotUI.SetItem(item.Key, item.Value);
        }
    }

}
