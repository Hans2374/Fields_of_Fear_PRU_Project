using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // References to managers
    [HideInInspector] public CurrencyManager currencyManager;
    [HideInInspector] public ShopSystem shopSystem;
    [HideInInspector] public CropValueManager cropValueManager;
    [HideInInspector] public CarPartManager carPartManager;

    // Simplified game state tracking
    private bool isPlayerDead = false;

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

        // Find manager references
        FindManagers();
    }

    private void Start()
    {
        // Subscribe to scene loading events to find managers in new scenes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Handle scene changes
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find managers in the new scene
        FindManagers();
    }

    // Find all system managers in the scene
    private void FindManagers()
    {
        // Find currency manager
        if (currencyManager == null)
        {
            currencyManager = FindObjectOfType<CurrencyManager>();
        }

        // Find shop system
        if (shopSystem == null)
        {
            shopSystem = FindObjectOfType<ShopSystem>();
        }

        // Find crop value manager
        if (cropValueManager == null)
        {
            cropValueManager = FindObjectOfType<CropValueManager>();
        }

        // Find car part manager
        if (carPartManager == null)
        {
            carPartManager = FindObjectOfType<CarPartManager>();
        }
    }

    // Handle player death - now just loads game over scene
    public void OnPlayerDeath()
    {
        if (isPlayerDead) return; // Prevent multiple calls

        isPlayerDead = true;

        // Make sure time scale is normal before scene transition
        Time.timeScale = 1f;

        // Load the game over scene - adjust index as needed
        SceneManager.LoadScene(3); // Assuming 3 is your game over scene
    }

    // Restart the game
    public void RestartGame()
    {
        // Reset time scale
        Time.timeScale = 1f;

        // Reset game state
        isPlayerDead = false;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Return to main menu
    public void ReturnToMainMenu()
    {
        // Reset time scale
        Time.timeScale = 1f;

        // Reset game state
        isPlayerDead = false;

        // Load main menu scene (assuming it's scene 0)
        SceneManager.LoadScene(0);
    }

    // Buy a car part (called from shop)
    public void BuyCarPart(int price)
    {
        if (currencyManager != null && carPartManager != null)
        {
            if (currencyManager.TrySpendMoney(price))
            {
                carPartManager.AddCarPart();
                Debug.Log($"Bought car part for {price}");
            }
            else
            {
                Debug.Log("Not enough money to buy car part!");
            }
        }
    }

    // When a crop is harvested, this gets called
    public void OnCropHarvested(string cropName, Vector3 position)
    {
        if (cropValueManager != null)
        {
            cropValueManager.SellCrop(cropName, position);
        }
    }
}