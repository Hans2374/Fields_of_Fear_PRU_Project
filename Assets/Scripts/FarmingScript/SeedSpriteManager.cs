using System.Collections.Generic;
using UnityEngine;

public class SeedSpriteManager : MonoBehaviour
{
    // Singleton pattern
    public static SeedSpriteManager Instance { get; private set; }

    [Header("Crop Growth Sprites")]
    [Tooltip("Drag the 3 growth stage sprites for Carrot")]
    public Sprite[] carrotStages;

    [Tooltip("Drag the 3 growth stage sprites for Tomato")]
    public Sprite[] tomatoStages;

    [Tooltip("Drag the 3 growth stage sprites for Berry")]
    public Sprite[] berryStages;

    [Tooltip("Drag the 3 growth stage sprites for Grape")]
    public Sprite[] grapeStages;

    [Tooltip("Drag the 3 growth stage sprites for Potato")]
    public Sprite[] potatoStages;

    [Tooltip("Drag the 3 growth stage sprites for Radish")]
    public Sprite[] radishStages;

    [Tooltip("Drag the 3 growth stage sprites for Cabbage")]
    public Sprite[] cabbageStages;

    // Dictionary mapping seed types to their growth stage sprites
    private Dictionary<Item.ItemType, Sprite[]> seedToSpritesMap = new Dictionary<Item.ItemType, Sprite[]>();

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize the sprite mapping
        InitializeSpriteMapping();
    }

    private void InitializeSpriteMapping()
    {
        // Map each seed type to its growth stage sprites
        seedToSpritesMap[Item.ItemType.CarrotSeed] = carrotStages;
        seedToSpritesMap[Item.ItemType.TomatoSeed] = tomatoStages;
        seedToSpritesMap[Item.ItemType.BerrySeed] = berryStages;
        seedToSpritesMap[Item.ItemType.GrapeSeed] = grapeStages;
        seedToSpritesMap[Item.ItemType.PotatoSeed] = potatoStages;
        seedToSpritesMap[Item.ItemType.RadishSeed] = radishStages;
        seedToSpritesMap[Item.ItemType.CabbageSeed] = cabbageStages;

        // Log the initialization
        Debug.Log($"SeedSpriteManager initialized with {seedToSpritesMap.Count} seed types.");

        // Warn about any missing sprite arrays
        foreach (var entry in seedToSpritesMap)
        {
            if (entry.Value == null || entry.Value.Length == 0)
            {
                Debug.LogWarning($"No growth stage sprites set for {entry.Key}!");
            }
        }
    }

    // Call this method to set up a seed with the correct sprites and crop data
    public void SetupSeedItem(Item seedItem)
    {
        if (seedItem == null)
        {
            Debug.LogError("Cannot setup a null seed item!");
            return;
        }

        // 1. Create CropData if it doesn't exist
        if (seedItem.crop == null)
        {
            seedItem.crop = CreateCropData(seedItem.itemType);
        }

        // 2. Assign growth stage sprites
        if (seedToSpritesMap.TryGetValue(seedItem.itemType, out Sprite[] sprites))
        {
            if (sprites != null && sprites.Length > 0)
            {
                // Set growth stages on the crop data
                seedItem.crop.growthStages = sprites;

                // Also set it on the item directly for immediate access
                seedItem.growthStages = sprites;

                Debug.Log($"Successfully assigned {sprites.Length} growth stage sprites to {seedItem.itemType}");
            }
            else
            {
                Debug.LogWarning($"Growth stage sprites array is empty for {seedItem.itemType}");
            }
        }
        else
        {
            Debug.LogWarning($"No growth stage sprites found for {seedItem.itemType}");
        }
    }

    // Create a new CropData instance for a seed type
    private CropData CreateCropData(Item.ItemType seedType)
    {
        // Create CropData
        CropData cropData = ScriptableObject.CreateInstance<CropData>();

        // Set basic properties based on seed type
        cropData.cropName = seedType.ToString().Replace("Seed", "");

        // Set growth time and sell price based on seed type (you can customize these values)
        switch (seedType)
        {
            case Item.ItemType.CarrotSeed:
                cropData.timeToGrow = 3f;
                cropData.sellPrice = 20;
                break;
            case Item.ItemType.TomatoSeed:
                cropData.timeToGrow = 7f;
                cropData.sellPrice = 30;
                break;
            case Item.ItemType.BerrySeed:
                cropData.timeToGrow = 5f;
                cropData.sellPrice = 15;
                break;
            case Item.ItemType.GrapeSeed:
                cropData.timeToGrow = 6f;
                cropData.sellPrice = 20;
                break;
            case Item.ItemType.PotatoSeed:
                cropData.timeToGrow = 7f;
                cropData.sellPrice = 35;
                break;
            case Item.ItemType.RadishSeed:
                cropData.timeToGrow = 4f;
                cropData.sellPrice = 25;
                break;
            case Item.ItemType.CabbageSeed:
                cropData.timeToGrow = 6f;
                cropData.sellPrice = 30;
                break;
            default:
                cropData.timeToGrow = 5f;
                cropData.sellPrice = 10;
                break;
        }

        // Set growth stages if available
        if (seedToSpritesMap.TryGetValue(seedType, out Sprite[] sprites))
        {
            cropData.growthStages = sprites;
        }

        return cropData;
    }
}