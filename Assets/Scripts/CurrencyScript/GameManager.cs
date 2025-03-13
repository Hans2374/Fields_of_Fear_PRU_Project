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

    // Game state tracking
    private bool isGamePaused = false;
    private bool isPlayerDead = false;

    // UI References
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverScreen;

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

        // Initialize UI
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
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

    private void Update()
    {
        // Handle pause menu
        if (Input.GetKeyDown(KeyCode.Escape) && !isPlayerDead)
        {
            TogglePauseMenu();
        }
    }

    // Toggle the pause menu
    public void TogglePauseMenu()
    {
        isGamePaused = !isGamePaused;

        // Set time scale
        Time.timeScale = isGamePaused ? 0f : 1f;

        // Show/hide pause menu
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isGamePaused);
        }
    }

    // Handle player death
    public void OnPlayerDeath()
    {
        isPlayerDead = true;

        // Show game over screen
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
    }

    // Restart the game
    public void RestartGame()
    {
        // Reset time scale
        Time.timeScale = 1f;

        // Reset game state
        isGamePaused = false;
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
        isGamePaused = false;
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