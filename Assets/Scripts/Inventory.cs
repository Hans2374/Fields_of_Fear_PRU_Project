using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{

    public event EventHandler OnItemListChanged;

    private List<Item> items;

    public Inventory()
    {
        items = new List<Item>();
        Debug.Log("Inventory created");
        //AddItem(new Item {  itemType = Item.ItemType.BerrySeed, amount = 1 });
        //AddItem(new Item {  itemType = Item.ItemType.CarrotSeed, amount = 1 });
        //AddItem(new Item { itemType = Item.ItemType.GrapeSeed, amount = 1 });
        //AddItem(new Item { itemType = Item.ItemType.GrapeSeed, amount = 1 });
        //AddItem(new Item { itemType = Item.ItemType.GrapeSeed, amount = 1 });
        //AddItem(new Item { itemType = Item.ItemType.GrapeSeed, amount = 1 });
        //AddItem(new Item { itemType = Item.ItemType.GrapeSeed, amount = 1 });
        //AddItem(new Item { itemType = Item.ItemType.GrapeSeed, amount = 1 });
        AddItem(new Item{itemType = Item.ItemType.CarPart, amount = 1});
        Debug.Log($"Inventory has {items.Count} items");
    }

    public void AddItem(Item item)
    {
        // Check if the item needs sprites
        if (item.crop == null || item.growthStages == null || item.growthStages.Length == 0)
        {
            // Use UnityEngine.Object.FindObjectOfType instead of just FindObjectOfType
            SeedSpriteManager seedSpriteManager = UnityEngine.Object.FindObjectOfType<SeedSpriteManager>();
            if (seedSpriteManager != null)
            {
                seedSpriteManager.SetupSeedItem(item);
                Debug.Log($"Set up missing sprites/crop data for {item.itemType} during inventory add");
            }
        }

        if (item.crop == null)
        {
            Debug.LogWarning($"⚠ Item {item.itemType} chưa có dữ liệu cây trồng!");
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty);
        if (item.IsStackable())
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
