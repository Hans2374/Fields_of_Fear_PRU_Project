using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance; // Singleton để dễ truy cập từ các script khác
    private Dictionary<ItemType, int> items = new Dictionary<ItemType, int>(); // Lưu số lượng item theo loại

    public event Action OnInventoryChanged; // Sự kiện để cập nhật UI

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddItem(ItemType itemType)
    {
        if (items.ContainsKey(itemType))
        {
            items[itemType]++; // Nếu item đã có, tăng số lượng
        }
        else
        {
            items[itemType] = 1; // Nếu item chưa có, thêm vào với số lượng 1
        }

        Debug.Log($"Đã nhặt {itemType}, số lượng: {items[itemType]}");

        // Gọi sự kiện cập nhật UI
        OnInventoryChanged?.Invoke();
    }

    public Dictionary<ItemType, int> GetItems()
    {
        return new Dictionary<ItemType, int>(items);
    }
}
