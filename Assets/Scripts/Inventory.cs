using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance; // Singleton để dễ truy cập từ các script khác
    public Dictionary<ItemType, int> items = new Dictionary<ItemType, int>(); // Lưu số lượng item theo loại
    public int maxSlots = 9; // Số ô tối đa trong inventory

    public event Action OnInventoryChanged; // Sự kiện để cập nhật UI

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("✅ Inventory đã được khởi tạo.");
        }
        else
        {
            Debug.LogError("⚠ Inventory bị xóa do đã có một Instance khác!");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeEmptySlots();
        OnInventoryChanged?.Invoke(); // Gọi sự kiện cập nhật UI ngay sau khi game khởi động
    }

    private void InitializeEmptySlots()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            items[(ItemType)i] = 0; // Tạo ô trống
        }
        Debug.Log("📌 Inventory đã tạo sẵn các ô trống.");
    }

    public void AddItem(ItemType itemType)
    {
        if (items.ContainsKey(itemType))
        {
            items[itemType]++; // Nếu item đã có, tăng số lượng
        }
        else
        {
            items[itemType] = 1; // Nếu item mới, thêm vào
        }

        Debug.Log($"📦 {itemType} hiện có: {items[itemType]}");
        OnInventoryChanged?.Invoke(); // Gọi sự kiện cập nhật UI
    }


    public Dictionary<ItemType, int> GetItems()
    {
        return new Dictionary<ItemType, int>(items);
    }
}
