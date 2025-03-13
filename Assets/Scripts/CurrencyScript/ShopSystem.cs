using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ShopSystem : MonoBehaviour
{
    // Singleton instance
    public static ShopSystem Instance { get; private set; }

    // UI References
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform seedsContainer;
    [SerializeField] private Transform carPartsContainer;
    [SerializeField] private GameObject seedItemPrefab;
    [SerializeField] private GameObject carPartItemPrefab;
    [SerializeField] private TextMeshProUGUI clockText;
    [SerializeField] private TextMeshProUGUI closingTimeText;
    [SerializeField] private GameObject notEnoughMoneyMessage;

    // Shop availability
    [SerializeField] private WorldTime worldTime;
    [SerializeField] private float openHour = 6.0f; // 6 AM
    [SerializeField] private float closeHour = 18.0f; // 6 PM
    private bool isShopOpen = false;

    // Audio reference
    private AudioManager audioManager;

    // Reference to player's inventory
    private Inventory playerInventory;

    // Car parts tracking
    private int nextCarPartPrice = 100; // Start with the first part price
    private int carPartsAvailable = 1; // Only one car part available per day

    // Seed pricing (from the main document)
    private Dictionary<Item.ItemType, SeedInfo> seedInfoMap = new Dictionary<Item.ItemType, SeedInfo>();

    [Serializable]
    private class SeedInfo
    {
        public Item.ItemType itemType;
        public float probability; // Probability of appearing in shop
        public int harvestValue; // How much money one crop gives
        public int growthCycles; // How many times it can be harvested
        public int growthTimeDays; // Days to grow
        public int harvestTimeDays; // Days until harvestable
        public int regrowthTimeDays; // Days to regrow after harvest
    }

    [SerializeField] private List<SeedInfo> seedInfoList;

    // Shop inventory
    private List<Item.ItemType> availableSeeds = new List<Item.ItemType>();
    private int bundlePrice = 80; // Price for 5 random seeds

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize seed info map
        foreach (SeedInfo info in seedInfoList)
        {
            seedInfoMap[info.itemType] = info;
        }

        // Get audio manager
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();

        // Close shop panel initially
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        // Hide "not enough money" message
        if (notEnoughMoneyMessage != null)
        {
            notEnoughMoneyMessage.SetActive(false);
        }
    }

    private void Start()
    {
        // Subscribe to world time changes to monitor shop hours
        if (worldTime != null)
        {
            worldTime.WorldTimeChanged += OnWorldTimeChanged;
        }

        // Generate initial shop inventory
        RefreshShopInventory();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (worldTime != null)
        {
            worldTime.WorldTimeChanged -= OnWorldTimeChanged;
        }
    }

    // Monitor the time to open/close shop
    private void OnWorldTimeChanged(object sender, TimeSpan newTime)
    {
        float currentHour = (float)newTime.TotalHours % 24;

        // Check if shop should be open
        bool shouldBeOpen = (currentHour >= openHour && currentHour < closeHour);

        // Handle shop opening
        if (shouldBeOpen && !isShopOpen)
        {
            OpenShop();

            // Play bell sound
                audioManager.PlaySFX(audioManager.bellDoor); // Replace with a bell sound
          

            // Refresh inventory at the start of each day
            RefreshShopInventory();
        }
        // Handle shop closing
        else if (!shouldBeOpen && isShopOpen)
        {
            CloseShop();
        }

        // Update closing time countdown
        UpdateClosingTime(currentHour);
    }

    // Update the closing time display
    private void UpdateClosingTime(float currentHour)
    {
        if (closingTimeText == null || !isShopOpen) return;

        float hoursRemaining = closeHour - currentHour;
        if (hoursRemaining < 0) hoursRemaining += 24;

        int hours = Mathf.FloorToInt(hoursRemaining);
        int minutes = Mathf.FloorToInt((hoursRemaining - hours) * 60);

        closingTimeText.text = $"Closing in: {hours}h {minutes}m";
    }

    // Refresh the shop's inventory (call daily)
    private void RefreshShopInventory()
    {
        // Clear previous inventory
        availableSeeds.Clear();

        // Randomly select 3 seed types based on probabilities
        List<Item.ItemType> allSeedTypes = new List<Item.ItemType>(seedInfoMap.Keys);

        // Sort seeds by probability (this simulates weighted random selection)
        allSeedTypes.Sort((a, b) =>
        {
            float probA = seedInfoMap[a].probability;
            float probB = seedInfoMap[b].probability;
            return probB.CompareTo(probA); // Descending order
        });

        // Take the first 3 (or fewer if we don't have enough types)
        int seedCount = Mathf.Min(3, allSeedTypes.Count);
        for (int i = 0; i < seedCount; i++)
        {
            availableSeeds.Add(allSeedTypes[i]);
        }

        // Increase car part price according to the progression
        // This matches the pricing in the document: 100, 150, 200, 300, 400, 500, 650, 800, 1000, 1200
        switch (nextCarPartPrice)
        {
            case 100: nextCarPartPrice = 150; break;
            case 150: nextCarPartPrice = 200; break;
            case 200: nextCarPartPrice = 300; break;
            case 300: nextCarPartPrice = 400; break;
            case 400: nextCarPartPrice = 500; break;
            case 500: nextCarPartPrice = 650; break;
            case 650: nextCarPartPrice = 800; break;
            case 800: nextCarPartPrice = 1000; break;
            case 1000: nextCarPartPrice = 1200; break;
            default: nextCarPartPrice = 1200; break; // Cap at 1200
        }

        // Reset available car parts
        carPartsAvailable = 1;

        Debug.Log($"Shop refreshed with {availableSeeds.Count} seed types and 1 car part for {nextCarPartPrice}");
    }

    // Open the shop UI
    public void OpenShop()
    {
        if (shopPanel == null) return;

        isShopOpen = true;
        shopPanel.SetActive(true);

        // Find player inventory if not set
        if (playerInventory == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Note: In your game structure, you might need to adjust how to get the inventory
                // This is just an example approach
                playerInventory = new Inventory(); // Or however you access the player's inventory
            }
        }

        // Populate shop UI
        PopulateShopUI();

        // Play sound
        
            audioManager.PlaySFX(audioManager.bellDoor);
      
    }

    // Close the shop UI
    public void CloseShop()
    {
        if (shopPanel == null) return;

        isShopOpen = false;
        shopPanel.SetActive(false);

        // Play sound
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.bellDoor);
        }
    }

    // Populate the shop UI with available items
    private void PopulateShopUI()
    {
        // Clear existing items
        foreach (Transform child in seedsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in carPartsContainer)
        {
            Destroy(child.gameObject);
        }

        // Add seed bundle
        CreateShopItem(seedsContainer, seedItemPrefab, "Seed Bundle (5 random)", bundlePrice, () => BuySeedBundle());

        // Add individual seeds with higher prices
        foreach (Item.ItemType seedType in availableSeeds)
        {
            string seedName = seedType.ToString().Replace("Seed", "");
            int seedPrice = bundlePrice / 4; // Individual seeds cost more than the bundle per seed

            CreateShopItem(seedsContainer, seedItemPrefab, seedName, seedPrice, () => BuySeed(seedType));
        }

        // Add car part if available
        if (carPartsAvailable > 0)
        {
            CreateShopItem(carPartsContainer, carPartItemPrefab, "Car Part", nextCarPartPrice, () => BuyCarPart());
        }
    }

    // Create a shop item UI element
    private void CreateShopItem(Transform parent, GameObject prefab, string itemName, int price, Action onBuyClicked)
    {
        GameObject itemObj = Instantiate(prefab, parent);

        // Set item name
        TextMeshProUGUI nameText = itemObj.transform.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
        if (nameText != null)
        {
            nameText.text = itemName;
        }

        // Set price
        TextMeshProUGUI priceText = itemObj.transform.Find("Price")?.GetComponent<TextMeshProUGUI>();
        if (priceText != null)
        {
            priceText.text = price.ToString();
        }

        // Set buy button
        Button buyButton = itemObj.transform.Find("BuyButton")?.GetComponent<Button>();
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(() => onBuyClicked?.Invoke());
        }
    }

    // Buy a seed bundle (5 random seeds)
    public void BuySeedBundle()
    {
        if (CurrencyManager.Instance == null || !CurrencyManager.Instance.CanAfford(bundlePrice))
        {
            ShowNotEnoughMoneyMessage();
            return;
        }

        // Spend money
        if (CurrencyManager.Instance.TrySpendMoney(bundlePrice))
        {
            // Generate 5 random seeds weighted by probability
            List<Item> seeds = GenerateRandomSeeds(5);

            // Add to player inventory
            if (playerInventory != null)
            {
                foreach (Item seed in seeds)
                {
                    playerInventory.AddItem(seed);
                }

                // Play success sound
                if (audioManager != null)
                {
                    audioManager.PlaySFX(audioManager.moneySpend);
                }

                Debug.Log($"Purchased seed bundle for {bundlePrice}");
            }
            else
            {
                Debug.LogError("Player inventory not found!");
            }
        }
    }

    // Buy a single seed
    public void BuySeed(Item.ItemType seedType)
    {
        int seedPrice = bundlePrice / 4;

        if (CurrencyManager.Instance == null || !CurrencyManager.Instance.CanAfford(seedPrice))
        {
            ShowNotEnoughMoneyMessage();
            return;
        }

        // Spend money
        if (CurrencyManager.Instance.TrySpendMoney(seedPrice))
        {
            // Create seed item
            Item seedItem = new Item
            {
                itemType = seedType,
                amount = 1
            };

            // Add to player inventory
            if (playerInventory != null)
            {
                playerInventory.AddItem(seedItem);

                // Play success sound
                if (audioManager != null)
                {
                    audioManager.PlaySFX(audioManager.moneySpend);
                }

                Debug.Log($"Purchased {seedType} for {seedPrice}");
            }
            else
            {
                Debug.LogError("Player inventory not found!");
            }
        }
    }

    // Buy a car part
    public void BuyCarPart()
    {
        if (CurrencyManager.Instance == null || !CurrencyManager.Instance.CanAfford(nextCarPartPrice))
        {
            ShowNotEnoughMoneyMessage();
            return;
        }

        // Spend money
        if (CurrencyManager.Instance.TrySpendMoney(nextCarPartPrice))
        {
            // Reduce available parts
            carPartsAvailable--;

            // TODO: Add car part to player's collection
            // This depends on how you're tracking car parts in your game
            // For example:
            // GameManager.Instance.AddCarPart();

            // Play success sound
            if (audioManager != null)
            {
                audioManager.PlaySFX(audioManager.moneySpend);
            }

            Debug.Log($"Purchased car part for {nextCarPartPrice}");

            // Update UI
            PopulateShopUI();
        }
    }

    // Show "not enough money" message
    private void ShowNotEnoughMoneyMessage()
    {
        if (notEnoughMoneyMessage == null) return;

        notEnoughMoneyMessage.SetActive(true);
        StartCoroutine(HideMessageAfterDelay(2.0f));
    }

    // Hide message after delay
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (notEnoughMoneyMessage != null)
        {
            notEnoughMoneyMessage.SetActive(false);
        }
    }

    // Generate random seeds based on probabilities
    private List<Item> GenerateRandomSeeds(int count)
    {
        List<Item> seeds = new List<Item>();
        List<Item.ItemType> allSeedTypes = new List<Item.ItemType>(seedInfoMap.Keys);

        for (int i = 0; i < count; i++)
        {
            // Simple weighted random selection
            float totalProb = 0f;
            foreach (var type in allSeedTypes)
            {
                totalProb += seedInfoMap[type].probability;
            }

            float randomVal = UnityEngine.Random.Range(0f, totalProb);
            float cumulativeProb = 0f;

            Item.ItemType selectedType = allSeedTypes[0]; // Default

            foreach (var type in allSeedTypes)
            {
                cumulativeProb += seedInfoMap[type].probability;
                if (randomVal <= cumulativeProb)
                {
                    selectedType = type;
                    break;
                }
            }

            // Create the seed item
            Item seedItem = new Item
            {
                itemType = selectedType,
                amount = 1
            };

            seeds.Add(seedItem);
        }

        return seeds;
    }

    // Check if shop is currently open
    public bool IsShopOpen()
    {
        return isShopOpen;
    }
}