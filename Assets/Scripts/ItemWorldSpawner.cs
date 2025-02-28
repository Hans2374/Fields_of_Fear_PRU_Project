using UnityEngine;
using static Item;

public class ItemWorldSpawner : MonoBehaviour
{
    private void Awake()
    {
        if (ItemAssets.Instance == null)
        {
            Debug.LogError("🚨 `ItemAssets.Instance` vẫn null! Kiểm tra lại xem `ItemAssets` đã được gán đúng chưa.");
            return;
        }

        Vector3 spawnPosition = new Vector3(-30, -12, 0);
        ItemWorld.SpawnItemWorld(spawnPosition, new Item { itemType = ItemType.BerrySeed, amount = 1 });
    }
}
