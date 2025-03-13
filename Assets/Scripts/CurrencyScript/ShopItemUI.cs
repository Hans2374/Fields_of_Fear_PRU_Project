using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopItemUI : MonoBehaviour
{
    // UI Components
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject outOfStockOverlay;
    [SerializeField] private GameObject cantAffordOverlay;

    // Item data
    private string itemName;
    private int price;
    private bool inStock = true;
    private Sprite itemSprite;

    // Action to call when purchased
    private Action onPurchased;

    // Audio for button clicks
    private AudioManager audioManager;

    private void Awake()
    {
        // Find audio manager
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();

        // Add click listener
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyButtonClicked);
        }
    }

    private void Start()
    {
        // Update UI with initial values
        UpdateUI();

        // Subscribe to currency changes to update affordability
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.OnMoneyChanged += OnMoneyChanged;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.OnMoneyChanged -= OnMoneyChanged;
        }
    }

    // Setup the shop item
    public void Setup(string name, int itemPrice, Sprite sprite, Action onPurchasedCallback, bool available = true)
    {
        itemName = name;
        price = itemPrice;
        itemSprite = sprite;
        onPurchased = onPurchasedCallback;
        inStock = available;

        // Update UI immediately
        UpdateUI();

        // Check if player can afford this
        CheckAffordability();
    }

    // Update UI elements based on current data
    private void UpdateUI()
    {
        // Set item name
        if (itemNameText != null)
        {
            itemNameText.text = itemName;
        }

        // Set price
        if (priceText != null)
        {
            priceText.text = price.ToString();
        }

        // Set image
        if (itemImage != null && itemSprite != null)
        {
            itemImage.sprite = itemSprite;
        }

        // Show/hide out of stock overlay
        if (outOfStockOverlay != null)
        {
            outOfStockOverlay.SetActive(!inStock);
        }

        // Enable/disable buy button
        if (buyButton != null)
        {
            buyButton.interactable = inStock;
        }
    }

    // Handle buy button click
    private void OnBuyButtonClicked()
    {
        if (!inStock) return;

        // Check if player can afford it
        if (CurrencyManager.Instance != null && CurrencyManager.Instance.CanAfford(price))
        {
            // Try to spend money
            if (CurrencyManager.Instance.TrySpendMoney(price))
            {
                // Play purchase sound
                if (audioManager != null)
                {
                    audioManager.PlaySFX(audioManager.moneySpend);
                }

                // Call purchase callback
                onPurchased?.Invoke();

                // Mark as out of stock if it's a one-time purchase (like car parts)
                if (itemName.Contains("Car Part"))
                {
                    inStock = false;
                    UpdateUI();
                }
            }
        }
        else
        {
            // Can't afford - show overlay briefly
            StartCoroutine(ShowCantAffordOverlay());
        }
    }

    // Called when player's money changes
    private void OnMoneyChanged(int newAmount)
    {
        CheckAffordability();
    }

    // Check if player can afford this item
    private void CheckAffordability()
    {
        if (!inStock) return;

        bool canAfford = CurrencyManager.Instance != null && CurrencyManager.Instance.CanAfford(price);

        // Update button interactability
        if (buyButton != null)
        {
            buyButton.interactable = canAfford && inStock;
        }

        // Show/hide can't afford overlay
        if (cantAffordOverlay != null)
        {
            cantAffordOverlay.SetActive(!canAfford && inStock);
        }
    }

    // Show the can't afford overlay briefly
    private System.Collections.IEnumerator ShowCantAffordOverlay()
    {
        if (cantAffordOverlay == null) yield break;

        cantAffordOverlay.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        cantAffordOverlay.SetActive(false);
    }
}