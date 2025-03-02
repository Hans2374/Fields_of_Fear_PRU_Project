using System;
using UnityEngine;


[Serializable]
public class Item
{
    public enum ItemType
    {
        BerrySeed,
        CarrotSeed,
        GrapeSeed,
        PotatoSeed,
        RadishSeed,
        CabbageSeed,
        TomatoSeed

    }


    
    public int amount;
    public ItemType itemType;

    public Item(ItemType itemType, int amount)
    {
        this.itemType = itemType;
        this.amount = amount;
    }

    public ItemData ToItemData()
    {
        return new ItemData(itemType, amount);
    }

    public static Item FromItemData(ItemData data)
    {
        return new Item(data.itemType, data.amount);
    }
    public Sprite GetSprite()
    {
        switch (itemType)
        {
            default:
            case ItemType.BerrySeed: return ItemAssets.Instance.BerrySeedSprite;
            case ItemType.CarrotSeed: return ItemAssets.Instance.CarrotSeedSprite;
            case ItemType.GrapeSeed: return ItemAssets.Instance.GrapeSeedSprite;
            case ItemType.PotatoSeed: return ItemAssets.Instance.PotatoSeedSprite;
            case ItemType.RadishSeed: return ItemAssets.Instance.RadishSeedSprite;
            case ItemType.CabbageSeed: return ItemAssets.Instance.CabbageSeedSprite;
            case ItemType.TomatoSeed: return ItemAssets.Instance.TomatoSeedSprite;
        }
    }

    public bool IsStackable()
    {
        switch (itemType)
        {
            default:
            case ItemType.BerrySeed:
            case ItemType.CarrotSeed:
            case ItemType.GrapeSeed:
            case ItemType.PotatoSeed:
            case ItemType.RadishSeed:
            case ItemType.CabbageSeed:
            case ItemType.TomatoSeed:
                return true;
        }
    }
}
