using UnityEngine;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private GameObject mainAreaContainer;
    [SerializeField] private GameObject shopAreaContainer;
    [SerializeField] private GameObject houseAreaContainer; // Added house area container

    // Audio manager reference
    private AudioManager audioManager;

    // Player reference - will be found dynamically
    private GameObject player;

    // Keep track of the current active area
    private string currentArea = "Main";

    // Getter for the current area (used by CameraController)
    public string GetCurrentArea()
    {
        return currentArea;
    }

    private void Awake()
    {
        Debug.Log("[TransitionManager] Awake called");

        // Verify important references
        if (mainAreaContainer == null) Debug.LogError("[TransitionManager] mainAreaContainer reference is missing!");
        if (shopAreaContainer == null) Debug.LogError("[TransitionManager] shopAreaContainer reference is missing!");
        if (houseAreaContainer == null) Debug.LogError("[TransitionManager] houseAreaContainer reference is missing!");
    }

    private void Start()
    {
        Debug.Log("[TransitionManager] Start called - Initializing scene");

        // Initialize the scene with Main area active and other areas inactive
        if (mainAreaContainer != null)
        {
            mainAreaContainer.SetActive(true);
            Debug.Log("[TransitionManager] Main area activated");
        }

        if (shopAreaContainer != null)
        {
            shopAreaContainer.SetActive(false);
            Debug.Log("[TransitionManager] Shop area deactivated");
        }

        if (houseAreaContainer != null)
        {
            houseAreaContainer.SetActive(false);
            Debug.Log("[TransitionManager] House area deactivated");
        }

        // Update camera controller
        UpdateCameraController();
    }

    // This method replaces the scene loading functionality
    public void TransitionToArea(string destinationArea, string spawnPointID)
    {
        Debug.Log($"[TransitionManager] TransitionToArea called: Destination={destinationArea}, SpawnID={spawnPointID}");

        // Start the transition coroutine
        StartCoroutine(PerformAreaTransition(destinationArea, spawnPointID));
    }

    private IEnumerator PerformAreaTransition(string destinationArea, string spawnPointID)
    {
        Debug.Log($"[TransitionManager] Starting transition from {currentArea} to {destinationArea}");

        // Always find the player at transition time to ensure we have the current instance
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[TransitionManager] Cannot find player with tag 'Player'!");
            yield break; // Exit if player not found
        }

        // Find spawn point and teleport player
        Debug.Log($"[TransitionManager] Looking for spawn point with ID: {spawnPointID}");
        PlayerSpawner targetSpawner = FindSpawnPoint(spawnPointID);
        if (targetSpawner != null && player != null)
        {
            Vector2 oldPosition = player.transform.position;

            // Disable character controller/rigidbody temporarily to avoid physics issues
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.simulated = false;
            }

            // Set position but maintain z-value to preserve 2D layering if needed
            float originalZ = player.transform.position.z;
            Vector3 newPosition = new Vector3(
                targetSpawner.transform.position.x,
                targetSpawner.transform.position.y,
                originalZ
            );

            // Force position change directly
            player.transform.position = newPosition;

            // For 2D, we only care about rotation around the z-axis
            player.transform.rotation = targetSpawner.transform.rotation;

            // Re-enable physics after position change
            if (playerRb != null)
            {
                playerRb.simulated = true;
            }

            Debug.Log($"[TransitionManager] Player teleported from {oldPosition} to {player.transform.position}");
        }
        else
        {
            if (targetSpawner == null) Debug.LogError("[TransitionManager] Transition failed: Spawn point not found!");
            if (player == null) Debug.LogError("[TransitionManager] Transition failed: Player reference is null!");
        }

        // Toggle area visibility based on destination
        Debug.Log($"[TransitionManager] Toggling area visibility for destination: {destinationArea}, current: {currentArea}");

        // Deactivate all areas first
        if (mainAreaContainer != null) mainAreaContainer.SetActive(false);
        if (shopAreaContainer != null) shopAreaContainer.SetActive(false);
        if (houseAreaContainer != null) houseAreaContainer.SetActive(false);

        // Activate only the destination area
        switch (destinationArea)
        {
            case "Main":
                if (mainAreaContainer != null) mainAreaContainer.SetActive(true);
                currentArea = "Main";
                Debug.Log("[TransitionManager] Main area activated");
                OnEnterMain();
                break;

            case "Shop":
                if (shopAreaContainer != null) shopAreaContainer.SetActive(true);
                currentArea = "Shop";
                Debug.Log("[TransitionManager] Shop area activated");

                // Find AudioManager right before using it if it's still null
                if (audioManager == null)
                {
                    audioManager = FindObjectOfType<AudioManager>();
                    if (audioManager == null)
                    {
                        Debug.LogError("[TransitionManager] Failed to find AudioManager before entering shop!");
                    }
                }

                OnEnterShop();
                break;

            case "House":
                if (houseAreaContainer != null) houseAreaContainer.SetActive(true);
                currentArea = "House";
                Debug.Log("[TransitionManager] House area activated");
                OnEnterHouse();
                break;

            default:
                Debug.LogError($"[TransitionManager] Unknown destination area: {destinationArea}");
                // Fallback to main area
                if (mainAreaContainer != null) mainAreaContainer.SetActive(true);
                currentArea = "Main";
                break;
        }

        // Update the camera with new area limits
        UpdateCameraController();

        // Wait a moment to ensure everything loads
        Debug.Log("[TransitionManager] Waiting for scene to stabilize...");
        yield return new WaitForSeconds(0.2f);

        Debug.Log($"[TransitionManager] Area transition complete. Current area: {currentArea}");
    }

    // Update the camera controller with the current area
    private void UpdateCameraController()
    {
        CameraController cameraController = FindObjectOfType<CameraController>();
        if (cameraController != null)
        {
            Debug.Log($"[TransitionManager] Updating camera controller with current area: {currentArea}");
            cameraController.SetAreaLimits(currentArea);
        }
        else
        {
            Debug.LogWarning("[TransitionManager] CameraController not found in scene");
        }
    }

    private PlayerSpawner FindSpawnPoint(string id)
    {
        // Find all spawn points in the scene
        PlayerSpawner[] allSpawners = FindObjectsOfType<PlayerSpawner>(true); // Include inactive objects
        Debug.Log($"[TransitionManager] Found {allSpawners.Length} spawn points in scene");

        // List all spawners for debugging
        foreach (PlayerSpawner sp in allSpawners)
        {
            Debug.Log($"[TransitionManager] - Spawn point: {sp.gameObject.name}, ID: {sp.SpawnPointID}, Position: {sp.transform.position}");
        }

        // Find the one with the matching ID
        foreach (PlayerSpawner spawner in allSpawners)
        {
            if (spawner.SpawnPointID == id)
            {
                Debug.Log($"[TransitionManager] Found matching spawn point: {spawner.gameObject.name} at {spawner.transform.position}");
                return spawner;
            }
        }

        Debug.LogError($"[TransitionManager] Spawn point with ID '{id}' not found!");
        return null;
    }

    // Area initialization methods
    private void OnEnterShop()
    {
        // Try one last time to get AudioManager if it's still null
        if (audioManager == null)
        {
            Debug.LogWarning("[TransitionManager] AudioManager still null in OnEnterShop, trying direct find...");
            // Try different ways to find AudioManager
            audioManager = FindObjectOfType<AudioManager>();

            // If that fails, try a broader search
            if (audioManager == null)
            {
                Debug.LogWarning("[TransitionManager] Trying broader search for AudioManager...");
                GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();
                foreach (GameObject go in allGameObjects)
                {
                    AudioManager am = go.GetComponent<AudioManager>();
                    if (am != null)
                    {
                        audioManager = am;
                        Debug.Log($"[TransitionManager] Found AudioManager on GameObject: {go.name}");
                        break;
                    }
                }
            }
        }

        // Play doorbell sound when entering the shop
        if (audioManager != null)
        {
            Debug.Log("[TransitionManager] Playing doorbell sound");
            try
            {
                audioManager.PlaySFX(audioManager.bellDoor);
                Debug.Log("[TransitionManager] Doorbell sound played successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[TransitionManager] Error playing doorbell sound: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("[TransitionManager] Cannot play doorbell - AudioManager is null after all attempts");
        }

        // For example: Update shop inventory, start dialogue, etc.
        Debug.Log("[TransitionManager] OnEnterShop called - performing shop initialization");
    }

    private void OnEnterMain()
    {
        // For example: Update quests, change music, etc.
        Debug.Log("[TransitionManager] OnEnterMain called - performing main area initialization");
    }

    private void OnEnterHouse()
    {
        // Play doorbell sound when entering the house
        if (audioManager != null)
        {
            Debug.Log("[TransitionManager] Playing doorbell sound");
            audioManager.PlaySFX(audioManager.bellDoor);
        }

        // Add house-specific initialization logic here
        Debug.Log("[TransitionManager] OnEnterHouse called - performing house initialization");
        // For example: Turn on lights, play house ambience, etc.
    }

    // Add this to your TransitionManager script for testing
    private void Update()
    {
        // Test transition to Shop
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("[TEST] Manually triggering transition to Shop");
            TransitionToArea("Shop", "MainToShop");
        }

        // Test transition to Main
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("[TEST] Manually triggering transition to Main");
            TransitionToArea("Main", "ShopToMain");
        }

        // Test transition to House
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log("[TEST] Manually triggering transition to House");
            TransitionToArea("House", "MainToHouse");
        }
    }
}