using UnityEngine;

[CreateAssetMenu(fileName = "New Item Price Data", menuName = "Shop/Item Price Data")]
public class ItemPrice : ScriptableObject
{
    // Dictionary-like structure mapping ItemType to price
    [System.Serializable]
    public class ItemPriceEntry
    {
        public Item.ItemType itemType;
        public int buyPrice;  // Price to buy FROM shop
        public int sellPrice; // Price to sell TO shop
    }

    // Array of all item prices
    public ItemPriceEntry[] itemPrices;

    // Default values for items not specified
    public int defaultBuyPrice = 100;
    public int defaultSellPrice = 50;

    // Get buy price for a specific item type
    public int GetBuyPrice(Item.ItemType itemType)
    {
        foreach (ItemPriceEntry entry in itemPrices)
        {
            if (entry.itemType == itemType)
                return entry.buyPrice;
        }

        // Return default price if not found
        return defaultBuyPrice;
    }

    // Get sell price for a specific item type
    public int GetSellPrice(Item.ItemType itemType)
    {
        foreach (ItemPriceEntry entry in itemPrices)
        {
            if (entry.itemType == itemType)
                return entry.sellPrice;
        }

        // Return default price if not found
        return defaultSellPrice;
    }

    // Extension method to get the buy price of an item
    public int GetBuyPrice(Item item)
    {
        return GetBuyPrice(item.itemType);
    }

    // Extension method to get the sell price of an item
    public int GetSellPrice(Item item)
    {
        return GetSellPrice(item.itemType);
    }
}