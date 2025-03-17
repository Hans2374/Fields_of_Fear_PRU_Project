using UnityEngine;
using TMPro;

public class ShopInteractableItem : MonoBehaviour
{
    AudioManager audioManager;
    [Header("Item Settings")]
    [SerializeField] private bool isCarPart = false;

    // Removed interaction prompt
    [SerializeField] private TextMeshProUGUI priceText;

    // References
    private ShopManager shopManager;
    private CurrencyManager currencyManager;

    // Player detection
    private bool playerInRange = false;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
    }

    private void Start()
    {
        // Find managers
        shopManager = GetComponentInParent<ShopManager>() ?? FindObjectOfType<ShopManager>();
        currencyManager = FindObjectOfType<CurrencyManager>();

        // Update price text format to include "Cost:" prefix
        UpdatePriceText();
    }

    private void Update()
    {
        // Check for interaction
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PurchaseItem();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // Removed showing interaction prompt
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // Removed hiding interaction prompt
        }
    }

    private void PurchaseItem()
    {
        if (shopManager == null)
        {
            Debug.LogWarning("Shop Manager not found!");
            return;
        }

        // Buy the appropriate item type
        if (isCarPart)
        {
            shopManager.BuyCarPart();
        }
        else
        {
            shopManager.BuySeedBag();
        }
    }

    // Update price text to show "[price] coins per car part" format
    private void UpdatePriceText()
    {
        if (priceText != null)
        {
            // Get the current price from text
            string currentText = priceText.text;
            int price;

            // Try to parse the price if it's just a number
            if (int.TryParse(currentText, out price))
            {
                if (isCarPart)
                {
                    priceText.text = $"{price} coins per car part";
                }
                else
                {
                    priceText.text = $"{price}";
                }
            }
            // If it already has formatting, leave non-car parts as is
            else if (!currentText.Contains("coins per car part") && isCarPart)
            {
                // Extract just the number if possible
                string numberPart = currentText.Replace("Cost:", "").Trim();
                if (int.TryParse(numberPart, out price))
                {
                    priceText.text = $"{price} coins per car part";
                }
                else
                {
                    // If we can't parse a number, just add the suffix
                    priceText.text = $"{currentText} coins per car part";
                }
            }

            // Make sure the text is visible
            priceText.gameObject.SetActive(true);

            // Fix any alpha issues
            Color textColor = priceText.color;
            if (textColor.a <= 0.01f)
            {
                textColor.a = 1f;
                priceText.color = textColor;
            }
        }
    }

    // Called by ShopManager to update the price display
    public void SetPrice(int price)
    {
        if (priceText != null)
        {
            if (isCarPart)
            {
                priceText.text = $"{price} coins per car part";
            }
            else
            {
                priceText.text = $"Cost: {price}";
            }
        }
    }

    // Called by animation events or other scripts
    public void ShowAvailablePrompt(bool canAfford)
    {
        // Just update text color based on affordability
        if (priceText != null)
        {
            priceText.color = canAfford ? Color.green : Color.red;
        }
    }
}