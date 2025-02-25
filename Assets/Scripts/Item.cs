using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData;

    private void Start()
    {
        if (itemData == null)
        {
            Debug.LogError($"❌ {gameObject.name} không có ItemData! Kiểm tra Prefab.");
        }
        else
        {
            Debug.Log($"✅ {gameObject.name} có ItemData: {itemData.itemName}");
        }
    }
}
