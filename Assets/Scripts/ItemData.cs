using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public int maxStack = 99; // S? l??ng t?i ?a c� th? ch?a
    public ItemType itemType;
}
