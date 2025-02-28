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
        Debug.Log($"Inventory has {items.Count} items");
    }

    public void AddItem(Item item)
    {
        items.Add(item);
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
        Debug.Log($"Added {item.itemType} to inventory");
    }

    public List<Item> GetItems()
    {
        return items;
    }
}
