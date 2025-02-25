using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    public Image itemIcon;
    public Text quantityText;

    public void SetItem(ItemType itemType, int quantity)
    {
        ItemData itemData = Resources.Load<ItemData>($"Items/{itemType}");
        if (itemData != null)
        {
            itemIcon.sprite = itemData.itemIcon;
            quantityText.text = quantity.ToString();
        }
    }
}
