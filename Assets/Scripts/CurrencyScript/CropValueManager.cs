using System.Collections.Generic;
using UnityEngine;

public class CropValueManager : MonoBehaviour
{
    // Singleton instance
    public static CropValueManager Instance { get; private set; }

    // Value table based on the document information
    [System.Serializable]
    public class CropInfo
    {
        public string cropName;
        public Item.ItemType seedType;
        public int harvestValue; // Value of harvesting one crop
        public int growthCycles; // How many times it can be harvested (1 for one-time, more for repeating)
    }

    [SerializeField] private CropInfo[] cropValues;

    // Dictionary for quick lookup
    private Dictionary<string, CropInfo> cropInfoMap = new Dictionary<string, CropInfo>();
    private Dictionary<Item.ItemType, CropInfo> seedToCropMap = new Dictionary<Item.ItemType, CropInfo>();

    // Default value if crop not found
    [SerializeField] private int defaultCropValue = 10;

    // For selling animation and feedback
    public GameObject coinPrefab;
    public float coinAnimDuration = 1.0f;
    private AudioManager audioManager;

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

        // Build lookup dictionaries
        foreach (CropInfo info in cropValues)
        {
            cropInfoMap[info.cropName] = info;
            seedToCropMap[info.seedType] = info;
        }

        // Find audio manager
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
    }

    // Get the value of a specific crop
    public int GetCropValue(string cropName)
    {
        if (cropInfoMap.TryGetValue(cropName, out CropInfo info))
        {
            return info.harvestValue;
        }

        Debug.LogWarning($"Crop '{cropName}' not found in value table. Using default value.");
        return defaultCropValue;
    }

    // Get the number of growth cycles for a crop
    public int GetGrowthCycles(string cropName)
    {
        if (cropInfoMap.TryGetValue(cropName, out CropInfo info))
        {
            return info.growthCycles;
        }

        Debug.LogWarning($"Crop '{cropName}' not found in value table. Using 1 growth cycle.");
        return 1;
    }

    // Get crop info from seed type
    public CropInfo GetCropInfoFromSeed(Item.ItemType seedType)
    {
        if (seedToCropMap.TryGetValue(seedType, out CropInfo info))
        {
            return info;
        }

        Debug.LogWarning($"Seed type '{seedType}' not mapped to any crop. Using default values.");
        return new CropInfo
        {
            cropName = "Unknown",
            seedType = seedType,
            harvestValue = defaultCropValue,
            growthCycles = 1
        };
    }

    // Sell a crop and get money
    public void SellCrop(string cropName, Vector3 cropPosition)
    {
        int value = GetCropValue(cropName);

        // Add money to the player's account
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddMoney(value);

            // Show visual feedback
            ShowCoinAnimation(cropPosition, value);

            // Play sound
            if (audioManager != null)
            {
                audioManager.PlaySFX(audioManager.crops); // You might want a specific selling sound
            }

            Debug.Log($"Sold {cropName} for {value} money");
        }
    }

    // Show coins floating up animation
    private void ShowCoinAnimation(Vector3 position, int value)
    {
        if (coinPrefab == null) return;

        // Create coin visual
        GameObject coinObj = Instantiate(coinPrefab, position, Quaternion.identity);

        // Set the text to show the value
        TextMesh textMesh = coinObj.GetComponentInChildren<TextMesh>();
        if (textMesh != null)
        {
            textMesh.text = "+" + value.ToString();
        }

        // Destroy after animation completes
        Destroy(coinObj, coinAnimDuration);
    }
}