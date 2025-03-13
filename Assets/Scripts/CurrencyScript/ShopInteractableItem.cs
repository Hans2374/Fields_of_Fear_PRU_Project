using UnityEngine;
using TMPro;

public class ShopInteractableItem : MonoBehaviour
{
    AudioManager audioManager;
    [Header("Item Settings")]
    [SerializeField] private bool isCarPart = false;
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private TextMeshProUGUI priceText;

    // References
    private ShopManager shopManager;
    private CurrencyManager currencyManager;

    // Player detection
    private bool playerInRange = false;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    private void Start()
    {
        // Find managers
        shopManager = GetComponentInParent<ShopManager>() ?? FindObjectOfType<ShopManager>();
        currencyManager = FindObjectOfType<CurrencyManager>();

        // Hide prompt initially
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
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

            // Show interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // Hide interaction prompt
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
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

    // Called by animation events or other scripts
    public void ShowAvailablePrompt(bool canAfford)
    {
        // You could show different prompts based on affordability
        // For example, change text color or message
        if (interactionPrompt != null)
        {
            if (priceText != null)
            {
                priceText.color = canAfford ? Color.green : Color.red;
            }
        }
    }
}