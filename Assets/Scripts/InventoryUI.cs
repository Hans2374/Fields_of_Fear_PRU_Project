using JetBrains.Annotations;
using System;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;
public class InventoryUI : MonoBehaviour
{
    private Inventory inventory;
    public Transform itemSlotContainer;
    public Transform itemSlotTemplate;
    private CharacterMovement player;
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
        this.inventory = InventoryManager.Instance.inventory;

        inventory.OnItemListChanged += Inventory_OnItemListChanged;
        RefreshInventoryItems();
    }

    private void Inventory_OnItemListChanged(object sender, EventArgs e)
    {
        RefreshInventoryItems();
    }

    private void RefreshInventoryItems()
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

            //itemSlotTransform.GetComponent<Button_UI>().ClickFunc = () =>
            //{
            //    //Use item
            //};
            
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
}
