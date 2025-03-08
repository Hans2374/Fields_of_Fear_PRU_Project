using CodeMonkey;
using JetBrains.Annotations;
using System;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public Transform itemSlotContainer;
    public Transform itemSlotTemplate;
    private CharacterMovement player;
    public Item selectedItem;
    private void Awake()
    {
        itemSlotContainer = transform.Find("ItemSlotContainer");
        if (itemSlotContainer == null)
        {
            Debug.LogError("Không tìm thấy ItemSlotContainer! Kiểm tra tên trong Hierarchy.");
        }

        itemSlotTemplate = itemSlotContainer?.Find("ItemSlotTemplate");
        if (itemSlotTemplate == null)
        {
            Debug.LogError("Không tìm thấy ItemSlotTemplate! Kiểm tra xem nó có bị ẩn không.");
        }
    }

    public void SetPlayer(CharacterMovement player)
    {
        this.player = player;
    }
    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;

        inventory.OnItemListChanged += Inventory_OnItemListChanged;
        RefreshInventoryItems();
    }

    private void Inventory_OnItemListChanged(object sender, EventArgs e)
    {
        RefreshInventoryItems();
    }

    public void SelectItem(Item item)
    {
        if (item != null)
        {
            selectedItem = item;
            Debug.Log($"✅ Đã chọn item: {item.itemType}, Số lượng: {item.amount}, CropData: {item.crop?.cropName ?? "NULL"}");
        }
        else
        {
            Debug.LogWarning("⚠ Không thể chọn item vì nó NULL!");
        }
    }


    public void RefreshInventoryItems()
    {
        foreach (Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
            //if (child != itemSlotTemplate)
            //{
            //    Destroy(child.gameObject);
            //}
        }
        // Xóa các item cũ trước khi cập nhật
        foreach (Transform child in itemSlotContainer)
        {
            if (child != itemSlotTemplate)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (Item item in inventory.GetItems())
        {
            RectTransform itemSlotTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotTransform.gameObject.SetActive(true);

            // ✅ Gán sự kiện click vào button của item slot
            Button button = itemSlotTransform.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners(); // Xóa event cũ để tránh lỗi click sai
                button.onClick.AddListener(() => SelectItem(item)); // Gán sự kiện chọn item
            }
            else
            {
                Debug.LogWarning("⚠ Item slot không có Button component!");
            }

            Image image = itemSlotTransform.Find("Image").GetComponent<Image>();
            image.sprite = item.GetSprite();
            TextMeshProUGUI text = itemSlotTransform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
            if (item.amount > 1)
            {
                text.SetText(item.amount.ToString());
            }
            else
            {
                text.SetText("");
            }

        }
    }

    // ✅ Cập nhật UI khi trồng cây (gọi từ FarmLandManager)
    public void UpdateInventoryUI()
    {
        RefreshInventoryItems();
    }
}
