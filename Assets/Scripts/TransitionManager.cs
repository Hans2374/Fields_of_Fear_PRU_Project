using UnityEngine;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private GameObject mainAreaContainer;
    [SerializeField] private GameObject shopAreaContainer;
    [SerializeField] private GameObject houseAreaContainer; // Added house area container

    // Player reference - will be found dynamically
    private GameObject player;

    // Add any transition effects here (fade, loading screen, etc.)
    [SerializeField] private CanvasGroup transitionOverlay;
    [SerializeField] private float transitionDuration = 1.0f;

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

        // Initialize transition overlay
        if (transitionOverlay != null)
        {
            transitionOverlay.alpha = 0;
            transitionOverlay.gameObject.SetActive(false);
            Debug.Log("[TransitionManager] Transition overlay initialized");
        }
        else
        {
            Debug.Log("[TransitionManager] No transition overlay assigned (optional)");
        }

        // Find and update camera controller
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

        // 1. Fade out (if transition overlay exists)
        if (transitionOverlay != null)
        {
            Debug.Log("[TransitionManager] Starting fade out");
            transitionOverlay.gameObject.SetActive(true);
            float elapsedTime = 0;
            while (elapsedTime < transitionDuration)
            {
                transitionOverlay.alpha = Mathf.Lerp(0, 1, elapsedTime / transitionDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transitionOverlay.alpha = 1;
            Debug.Log("[TransitionManager] Fade out complete");
        }

        // 2. Find spawn point and teleport player
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

        // 3. Toggle area visibility based on destination
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

        // 4. Wait a moment to ensure everything loads
        Debug.Log("[TransitionManager] Waiting for scene to stabilize...");
        yield return new WaitForSeconds(0.2f);

        // 5. Fade back in (if transition overlay exists)
        if (transitionOverlay != null)
        {
            Debug.Log("[TransitionManager] Starting fade in");
            float elapsedTime = 0;
            while (elapsedTime < transitionDuration)
            {
                transitionOverlay.alpha = Mathf.Lerp(1, 0, elapsedTime / transitionDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transitionOverlay.alpha = 0;
            transitionOverlay.gameObject.SetActive(false);
            Debug.Log("[TransitionManager] Fade in complete");
        }

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