using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Items")]
    [SerializeField] private GameObject seedBagObject;
    [SerializeField] private GameObject carPartObject;
    [SerializeField] private TextMeshProUGUI carPartPriceText;

    [Header("Item Settings")]
    [SerializeField] private int seedBagPrice = 80;
    [SerializeField] private int seedsPerBag = 5;

    // Car part prices from your table - verified against provided values
    // Index 0 = car part 1 (price 100), Index 1 = car part 2 (price 150), etc.
    private int[] carPartPrices = { 100, 150, 200, 300, 400, 500, 650, 800, 1000, 1200 };
    private int currentCarPartIndex = 0;

    // Seed probabilities from your table
    [System.Serializable]
    public class SeedType
    {
        public Item.ItemType type;
        public string displayName;
        public float probability;
    }

    [SerializeField] private List<SeedType> seedTypes = new List<SeedType>();

    // Track if car part was bought today
    private bool carPartBoughtToday = false;

    // References
    private CurrencyManager currencyManager;
    private WorldTime worldTime;
    private AudioManager audioManager;
    private CropDataManager cropDataManager;
    private SeedSpriteManager seedSpriteManager;

    private void Awake()
    {
        // Find necessary components
        currencyManager = FindObjectOfType<CurrencyManager>();
        worldTime = FindObjectOfType<WorldTime>();
        audioManager = FindObjectOfType<AudioManager>();
        seedSpriteManager = FindObjectOfType<SeedSpriteManager>();

        // If SeedSpriteManager doesn't exist, create it
        if (seedSpriteManager == null)
        {
            GameObject spriteManagerObj = new GameObject("SeedSpriteManager");
            seedSpriteManager = spriteManagerObj.AddComponent<SeedSpriteManager>();
            Debug.Log("Created SeedSpriteManager because it didn't exist");
        }

        // Setup seed types if not already set in inspector
        if (seedTypes.Count == 0)
        {
            // Add seed types with probabilities from your table
            seedTypes.Add(new SeedType { type = Item.ItemType.CarrotSeed, displayName = "Cà rốt", probability = 25f });
            seedTypes.Add(new SeedType { type = Item.ItemType.RadishSeed, displayName = "Củ cải đường", probability = 20f });
            seedTypes.Add(new SeedType { type = Item.ItemType.CabbageSeed, displayName = "Bắp cải", probability = 15f });
            seedTypes.Add(new SeedType { type = Item.ItemType.PotatoSeed, displayName = "Khoai tây", probability = 15f });
            seedTypes.Add(new SeedType { type = Item.ItemType.BerrySeed, displayName = "Dâu", probability = 10f });
            seedTypes.Add(new SeedType { type = Item.ItemType.GrapeSeed, displayName = "Nho", probability = 10f });
            seedTypes.Add(new SeedType { type = Item.ItemType.TomatoSeed, displayName = "Cà chua", probability = 5f });
        }
    }

    private void Start()
    {
        // Subscribe to day change events to reset car part availability
        if (worldTime != null)
        {
            worldTime.WorldTimeChanged += OnTimeChanged;
        }

        // Load saved car part index
        LoadCarPartProgress();

        // Update car part price display
        UpdateCarPartDisplay();

        // Create CropDataManager if it doesn't exist
        if (cropDataManager == null)
        {
            GameObject cropDataObj = new GameObject("CropDataManager");
            cropDataManager = cropDataObj.AddComponent<CropDataManager>();
            Debug.Log("Created CropDataManager because it didn't exist");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (worldTime != null)
        {
            worldTime.WorldTimeChanged -= OnTimeChanged;
        }
    }

    // Check for new day
    private void OnTimeChanged(object sender, System.TimeSpan newTime)
    {
        // Reset car part availability at 6 AM (start of shop day)
        if (newTime.Hours == 6 && newTime.Minutes == 0)
        {
            carPartBoughtToday = false;
            UpdateCarPartDisplay();
        }
    }

    // Buy seed bag method - called from interaction
    public void BuySeedBag()
    {
        if (currencyManager == null || !currencyManager.CanAfford(seedBagPrice))
        {
            Debug.Log("Not enough money to buy seeds!");
            PlayErrorSound();
            return;
        }

        // Try to spend money
        if (currencyManager.TrySpendMoney(seedBagPrice))
        {
            // Generate random seeds based on probabilities
            List<Item> seedItems = GenerateRandomSeeds(seedsPerBag);

            // Add seeds to player inventory
            CharacterMovement player = FindObjectOfType<CharacterMovement>();
            if (player != null)
            {
                // Add each seed to inventory
                foreach (Item seed in seedItems)
                {
                    // IMPORTANT: Set up the seed with proper sprites and crop data
                    if (seedSpriteManager != null)
                    {
                        seedSpriteManager.SetupSeedItem(seed);
                        Debug.Log($"Set up {seed.itemType} with sprites and crop data");
                    }
                    else
                    {
                        Debug.LogError("SeedSpriteManager is null! Seeds will not have proper sprites.");
                    }

                    // Add to inventory
                    AddItemToPlayerInventory(seed);
                }

                PlaySuccessSound();
                Debug.Log($"Bought {seedsPerBag} random seeds for {seedBagPrice}!");
            }
            else
            {
                Debug.LogWarning("Player not found!");
            }
        }
    }

    // Helper method to add item to player inventory
    private void AddItemToPlayerInventory(Item item)
    {
        if (item == null)
        {
            Debug.LogError("Trying to add null item to inventory!");
            return;
        }

        // Find the player
        CharacterMovement player = FindObjectOfType<CharacterMovement>();
        if (player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        // Get the inventory through various possible paths
        Inventory inventory = null;

        // Option 1: Try from GetInventory method if it exists
        var methodInfo = player.GetType().GetMethod("GetInventory");
        if (methodInfo != null)
        {
            inventory = methodInfo.Invoke(player, null) as Inventory;
            if (inventory != null)
            {
                Debug.Log("Found inventory via GetInventory method");
            }
        }

        // Option 2: Try getting inventory from player's InventoryUI component
        if (inventory == null)
        {
            InventoryUI inventoryUI = player.GetComponent<InventoryUI>();
            if (inventoryUI != null && inventoryUI.inventory != null)
            {
                inventory = inventoryUI.inventory;
                Debug.Log("Found inventory via InventoryUI component");
            }
        }

        // Option 3: Find any InventoryUI in the scene
        if (inventory == null)
        {
            InventoryUI anyInventoryUI = FindObjectOfType<InventoryUI>();
            if (anyInventoryUI != null && anyInventoryUI.inventory != null)
            {
                inventory = anyInventoryUI.inventory;
                Debug.Log("Found inventory via scene InventoryUI");
            }
        }

        // Option 4: Try accessing inventory directly on player
        if (inventory == null)
        {
            var field = player.GetType().GetField("inventory",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                inventory = field.GetValue(player) as Inventory;
                if (inventory != null)
                {
                    Debug.Log("Found inventory via direct field access");
                }
            }
        }

        // Finally add the item
        if (inventory != null)
        {
            inventory.AddItem(item);
            Debug.Log($"Successfully added {item.itemType} x{item.amount} to inventory!");
        }
        else
        {
            Debug.LogError("Failed to find player inventory!");
        }
    }

    // Buy car part method - called from interaction
    public void BuyCarPart()
    {
        // Removed the check for carPartBoughtToday to allow multiple purchases

        // Get current price
        int price = GetCurrentCarPartPrice();

        if (currencyManager == null || !currencyManager.CanAfford(price))
        {
            Debug.Log("Not enough money to buy car part!");
            PlayErrorSound();
            return;
        }

        // Try to spend money
        if (currencyManager.TrySpendMoney(price))
        {
            // Create car part item for inventory
            Item carPartItem = new Item
            {
                itemType = Item.ItemType.CarPart,
                amount = 1
            };

            // Add car part to player inventory
            AddItemToPlayerInventory(carPartItem);

            // Also notify Car Part Manager (for game progression)
            CarPartManager carPartManager = FindObjectOfType<CarPartManager>();
            if (carPartManager != null)
            {
                carPartManager.AddCarPart();
            }
            else
            {
                Debug.LogWarning("CarPartManager not found - car part progression might not update properly!");
            }

            // Update car part index for next purchase
            currentCarPartIndex = Mathf.Min(currentCarPartIndex + 1, carPartPrices.Length - 1);

            // We still track if bought today, but it won't affect visibility or ability to purchase
            carPartBoughtToday = true;

            // Save progress
            SaveCarPartProgress();

            // Update display
            UpdateCarPartDisplay();

            PlaySuccessSound();
            Debug.Log($"Bought car part for {price} and added to inventory!");
        }
    }

    // Generate random seeds based on probabilities from your table
    private List<Item> GenerateRandomSeeds(int count)
    {
        List<Item> seeds = new List<Item>();

        for (int i = 0; i < count; i++)
        {
            float totalProbability = 0f;
            foreach (SeedType seed in seedTypes)
            {
                totalProbability += seed.probability;
            }

            float random = Random.Range(0f, totalProbability);
            float currentProb = 0f;

            Item.ItemType selectedType = Item.ItemType.CarrotSeed; // Default

            foreach (SeedType seed in seedTypes)
            {
                currentProb += seed.probability;
                if (random <= currentProb)
                {
                    selectedType = seed.type;
                    break;
                }
            }

            // Create and add seed item
            Item newSeed = new Item
            {
                itemType = selectedType,
                amount = 1
            };

            // Apply crop data from our manager
            if (cropDataManager != null)
            {
                cropDataManager.ApplyCropDataToSeed(newSeed);
            }

            seeds.Add(newSeed);

            // Log for testing
            Debug.Log($"Generated seed: {selectedType}");
        }

        return seeds;
    }

    // Get current car part price
    private int GetCurrentCarPartPrice()
    {
        if (currentCarPartIndex < carPartPrices.Length)
        {
            return carPartPrices[currentCarPartIndex];
        }

        // If we've gone through all predefined prices, use the last one
        return carPartPrices[carPartPrices.Length - 1];
    }

    // Update car part visual state
    private void UpdateCarPartDisplay()
    {
        if (carPartObject != null)
        {
            // Always show car part object regardless of purchase status
            carPartObject.SetActive(true);
        }

        // Update price text with new format
        if (carPartPriceText != null)
        {
            int price = GetCurrentCarPartPrice();
            carPartPriceText.text = $"{price} coins per car part";
        }
    }

    // Save car part progress to PlayerPrefs
    private void SaveCarPartProgress()
    {
        PlayerPrefs.SetInt("CarPartIndex", currentCarPartIndex);
        PlayerPrefs.Save();
    }

    // Load car part progress from PlayerPrefs
    private void LoadCarPartProgress()
    {
        // Option 1: Always reset car part index on game restart
        currentCarPartIndex = 0;

        // Option 2: If you want to maintain progress between sessions, uncomment this code:
        /*
        if (PlayerPrefs.HasKey("CarPartIndex"))
        {
            currentCarPartIndex = PlayerPrefs.GetInt("CarPartIndex");

            // Ensure valid index
            currentCarPartIndex = Mathf.Clamp(currentCarPartIndex, 0, carPartPrices.Length - 1);
        }
        */

        // Optional: Delete the saved PlayerPrefs key completely to ensure a fresh start
        if (PlayerPrefs.HasKey("CarPartIndex"))
        {
            PlayerPrefs.DeleteKey("CarPartIndex");
        }
    }

    // Play success sound
    private void PlaySuccessSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.moneySpend);
        }
    }

    // Play error sound
    private void PlayErrorSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.carFail);
        }
    }
}