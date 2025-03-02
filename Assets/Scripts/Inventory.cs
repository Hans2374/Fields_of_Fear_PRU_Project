using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Item;

[Serializable]
public class InventorySaveData
{
    public List<ItemData> items;

    public InventorySaveData(Inventory inventory)
    {
        items = new List<ItemData>();
        foreach (Item item in inventory.GetItems())
        {
            items.Add(item.ToItemData());
        }
    }
}
public class Inventory
{

    public event EventHandler OnItemListChanged;

    private List<Item> items;

    public Inventory()
    {
        items = new List<Item>();
        Debug.Log("Inventory created");

        Debug.Log($"Inventory has {items.Count} items");
    }

    public void AddItem(Item item)
    {
        if(item.IsStackable())
        {
            bool itemAlreadyInInventory = false;
            foreach (Item inventoryItem in items)
            {
                if (inventoryItem.itemType == item.itemType)
                {
                    inventoryItem.amount += item.amount;
                    itemAlreadyInInventory = true;
                }
            }
            if (!itemAlreadyInInventory)
            {
                items.Add(item);
            }
        }
        else
            items.Add(item);
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
        Debug.Log($"Added {item.itemType} to inventory");
    }

    public void RemoveItem(Item item)
    {
        if (item.IsStackable())
        {
            Item itemInInventory = null;
            foreach (Item inventoryItem in items)
            {
                if (inventoryItem.itemType == item.itemType)
                {
                    inventoryItem.amount -= item.amount;
                    itemInInventory = inventoryItem;
                }
            }
            if (itemInInventory != null && itemInInventory.amount <=0)
            {
                items.Remove(itemInInventory);
            }
        }
        else
            items.Remove(item);
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
        Debug.Log($"Removed {item.itemType} from inventory");
    }
    public List<Item> GetItems()
    {
        return items;
    }
}
