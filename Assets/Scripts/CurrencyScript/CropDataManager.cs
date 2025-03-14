using System.Collections.Generic;
using UnityEngine;

// This script manages CropData creation and assignment for seeds
public class CropDataManager : MonoBehaviour
{
    // Singleton instance
    public static CropDataManager Instance { get; private set; }

    [Header("Default Crop Sprites")]
    [SerializeField] private Sprite[] carrotStages;
    [SerializeField] private Sprite[] tomatoStages;
    [SerializeField] private Sprite[] berryStages;
    [SerializeField] private Sprite[] grapeStages;
    [SerializeField] private Sprite[] potatoStages;
    [SerializeField] private Sprite[] radishStages;
    [SerializeField] private Sprite[] cabbageStages;

    // Dictionary to store crop data for each seed type
    private Dictionary<Item.ItemType, CropData> cropDataMap = new Dictionary<Item.ItemType, CropData>();

    private void Awake()
    {
        // Set up singleton
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

        // Initialize crop data for all seed types
        InitializeCropData();
    }

    private void InitializeCropData()
    {
        // Create crop data for each seed type
        CreateCropData(Item.ItemType.CarrotSeed, "Carrot", 20, 3f, carrotStages);
        CreateCropData(Item.ItemType.TomatoSeed, "Tomato", 30, 7f, tomatoStages);
        CreateCropData(Item.ItemType.BerrySeed, "Berry", 15, 5f, berryStages);
        CreateCropData(Item.ItemType.GrapeSeed, "Grape", 20, 6f, grapeStages);
        CreateCropData(Item.ItemType.PotatoSeed, "Potato", 35, 7f, potatoStages);
        CreateCropData(Item.ItemType.RadishSeed, "Radish", 25, 4f, radishStages);
        CreateCropData(Item.ItemType.CabbageSeed, "Cabbage", 30, 6f, cabbageStages);

        Debug.Log($"CropDataManager initialized with {cropDataMap.Count} crop types");
    }

    private void CreateCropData(Item.ItemType seedType, string cropName, int sellPrice, float timeToGrow, Sprite[] growthStages)
    {
        // Check if we have growth stages
        Sprite[] stages = growthStages;
        if (stages == null || stages.Length == 0)
        {
            // Use fallback sprites if none are provided
            Debug.LogWarning($"No growth stages provided for {cropName}. Using fallback sprites.");
            stages = CreateFallbackSprites();
        }

        // Create a new CropData instance
        CropData cropData = ScriptableObject.CreateInstance<CropData>();
        cropData.name = cropName;
        cropData.cropName = cropName;
        cropData.sellPrice = sellPrice;
        cropData.timeToGrow = timeToGrow;
        cropData.growthStages = stages;

        // Add to our dictionary
        cropDataMap[seedType] = cropData;
        Debug.Log($"Created CropData for {seedType}: {cropName}");
    }

    private Sprite[] CreateFallbackSprites()
    {
        // Create simple placeholder sprites
        // For a real game, you should replace this with actual sprites
        Sprite[] fallbackSprites = new Sprite[3];

        // Try to load some default sprites from resources
        fallbackSprites[0] = Resources.Load<Sprite>("Crops/seedling") ?? CreateDefaultSprite(Color.green);
        fallbackSprites[1] = Resources.Load<Sprite>("Crops/growing") ?? CreateDefaultSprite(Color.green);
        fallbackSprites[2] = Resources.Load<Sprite>("Crops/mature") ?? CreateDefaultSprite(Color.green);

        return fallbackSprites;
    }

    private Sprite CreateDefaultSprite(Color color)
    {
        // Create a simple colored sprite as a last resort
        Texture2D texture = new Texture2D(32, 32);
        for (int x = 0; x < 32; x++)
            for (int y = 0; y < 32; y++)
                texture.SetPixel(x, y, color);

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 32, 32), Vector2.one * 0.5f);
    }

    // Get crop data for a specific seed type
    public CropData GetCropData(Item.ItemType seedType)
    {
        if (cropDataMap.TryGetValue(seedType, out CropData cropData))
        {
            return cropData;
        }

        Debug.LogWarning($"No CropData found for {seedType}. Creating default data.");
        // Create a default crop data if none exists
        CreateCropData(seedType, seedType.ToString().Replace("Seed", ""), 10, 5f, null);
        return cropDataMap[seedType];
    }

    // Apply crop data to a seed item
    public void ApplyCropDataToSeed(Item seedItem)
    {
        if (seedItem == null)
        {
            Debug.LogError("Cannot apply CropData to null seed item");
            return;
        }

        CropData cropData = GetCropData(seedItem.itemType);
        seedItem.crop = cropData;

        Debug.Log($"Applied CropData '{cropData.cropName}' to {seedItem.itemType}");
    }
}