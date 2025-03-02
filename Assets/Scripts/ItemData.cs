using System;

[Serializable]
public class ItemData
{
    public Item.ItemType itemType;
    public int amount;

    public ItemData(Item.ItemType itemType, int amount)
    {
        this.itemType = itemType;
        this.amount = amount;
    }
}
