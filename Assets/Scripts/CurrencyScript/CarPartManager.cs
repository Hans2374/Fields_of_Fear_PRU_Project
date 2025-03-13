using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CarPartManager : MonoBehaviour
{
    // Singleton instance
    public static CarPartManager Instance { get; private set; }

    // Car parts tracking
    [SerializeField] private int totalPartsNeeded = 5;
    private int partsCollected = 0;

    // UI References
    [SerializeField] private TextMeshProUGUI partsCountText;
    [SerializeField] private Image repairProgressBar;
    [SerializeField] private GameObject repairPrompt;
    [SerializeField] private GameObject repairCompletedMessage;
    [SerializeField] private GameObject victoryScreen;

    // Car visuals
    [SerializeField] private SpriteRenderer carRenderer;
    [SerializeField] private Sprite brokenCarSprite;
    [SerializeField] private Sprite fixedCarSprite;

    // References
    private AudioManager audioManager;
    private bool isPlayerInRange = false;
    private bool isRepairInProgress = false;

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

        // Get references
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();

        // Initialize UI
        UpdatePartsCountUI();

        if (repairPrompt != null)
        {
            repairPrompt.SetActive(false);
        }

        if (repairCompletedMessage != null)
        {
            repairCompletedMessage.SetActive(false);
        }

        if (victoryScreen != null)
        {
            victoryScreen.SetActive(false);
        }

        if (repairProgressBar != null)
        {
            repairProgressBar.fillAmount = 0f;
        }

        // Load saved data
        LoadCarPartsData();

        // Update car visuals
        UpdateCarVisuals();
    }

    private void Update()
    {
        // Check for repair input
        if (isPlayerInRange && partsCollected > 0 && !isRepairInProgress && Input.GetKeyDown(KeyCode.E))
        {
            StartRepairProcess();
        }
    }

    // Trigger when player enters repair area
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            // Show prompt if player has parts and car isn't fixed
            if (partsCollected > 0 && partsCollected < totalPartsNeeded && !isRepairInProgress && repairPrompt != null)
            {
                repairPrompt.SetActive(true);
            }
        }
    }

    // Trigger when player leaves repair area
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            // Hide prompt
            if (repairPrompt != null)
            {
                repairPrompt.SetActive(false);
            }
        }
    }

    // Add a new car part to the player's collection
    public void AddCarPart()
    {
        partsCollected++;

        // Update UI
        UpdatePartsCountUI();

        // Save progress
        SaveCarPartsData();

        Debug.Log($"Added car part! {partsCollected}/{totalPartsNeeded}");

        // Play sound
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.carIncrease);
        }
    }

    // Start the car repair minigame
    private void StartRepairProcess()
    {
        if (partsCollected <= 0 || isRepairInProgress) return;

        // Set flag to prevent multiple repairs
        isRepairInProgress = true;

        // Hide prompt
        if (repairPrompt != null)
        {
            repairPrompt.SetActive(false);
        }

        // Disable player movement
        CharacterMovement playerMovement = GameObject.FindGameObjectWithTag("Player")?.GetComponent<CharacterMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Start repair coroutine
        StartCoroutine(RepairCarCoroutine());
    }

    // Coroutine to handle the repair process
    private IEnumerator RepairCarCoroutine()
    {
        // Duration in seconds
        float repairDuration = 30f;
        float calibrationInterval = 5f;
        float currentTime = 0f;
        int failedCalibrations = 0;

        // Reset progress bar
        if (repairProgressBar != null)
        {
            repairProgressBar.gameObject.SetActive(true);
            repairProgressBar.fillAmount = 0f;
        }

        // Play repair start sound
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.carMove);
        }

        // Repair loop
        while (currentTime < repairDuration)
        {
            // Update progress bar
            if (repairProgressBar != null)
            {
                repairProgressBar.fillAmount = currentTime / repairDuration;
            }

            // Check if it's time for calibration
            if (Mathf.FloorToInt(currentTime / calibrationInterval) !=
                Mathf.FloorToInt((currentTime + Time.deltaTime) / calibrationInterval))
            {
                // Run calibration minigame
                bool calibrationSuccess = false;

                // Get result from coroutine
                yield return RunCalibrationMinigame((result) => {
                    calibrationSuccess = result;
                });

                if (!calibrationSuccess)
                {
                    failedCalibrations++;

                    // Play failure sound
                    if (audioManager != null)
                    {
                        audioManager.PlaySFX(audioManager.carFail);
                    }

                    // If too many failures, attract monster
                    if (failedCalibrations >= 3)
                    {
                        Debug.Log("Too many failed calibrations! Monster alerted!");
                        // TODO: Alert monster via your monster system
                        break;
                    }
                }
            }

            // Increment time
            currentTime += Time.deltaTime;
            yield return null;
        }

        // Hide progress bar
        if (repairProgressBar != null)
        {
            repairProgressBar.gameObject.SetActive(false);
        }

        // Check if repair was successful (not interrupted by monster)
        if (currentTime >= repairDuration)
        {
            // Use a car part
            partsCollected--;
            SaveCarPartsData();

            // Update UI
            UpdatePartsCountUI();

            // Check if car is fully repaired
            if (partsCollected == 0 && CarRepairMiniGame.MinigameCompletionCount >= totalPartsNeeded)
            {
                // Car is fixed!
                OnCarFullyRepaired();
            }
            else
            {
                // Car is partially repaired
                // Play success sound
                if (audioManager != null)
                {
                    audioManager.PlaySFX(audioManager.carDone);
                }

                // Show message
                StartCoroutine(ShowTempMessage(repairCompletedMessage, 3f));
            }

            // Update car visuals based on repair progress
            UpdateCarVisuals();
        }

        // Re-enable player movement
        CharacterMovement playerMovement = GameObject.FindGameObjectWithTag("Player")?.GetComponent<CharacterMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        // Reset repair flag
        isRepairInProgress = false;
    }

    // Simple calibration minigame using callback instead of direct return
    private IEnumerator RunCalibrationMinigame(System.Action<bool> callback)
    {
        // In reality, you would use your existing CarRepairMiniGame here
        // This is just a simplified version for demonstration

        // Show calibration UI
        // calibrationUI.SetActive(true);

        // Wait for player input or timeout
        float timeout = 3.0f;
        float elapsed = 0f;
        bool success = false;

        while (elapsed < timeout)
        {
            // Check for successful calibration input
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Check if timing is good (would be your slider check)
                float randomChance = Random.value;
                success = (randomChance > 0.3f); // 70% chance of success
                break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Hide calibration UI
        // calibrationUI.SetActive(false);

        // Return result through callback
        callback(success);
    }

    // Called when the car is fully repaired
    private void OnCarFullyRepaired()
    {
        Debug.Log("Car fully repaired! Player wins!");

        // Play victory music
        if (audioManager != null)
        {
            audioManager.StopMusic();
            audioManager.PlaySFX(audioManager.carDone);
        }

        // Update car sprite
        if (carRenderer != null && fixedCarSprite != null)
        {
            carRenderer.sprite = fixedCarSprite;
        }

        // Show victory screen
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true);
        }

        // Load victory scene after delay
        StartCoroutine(EndGameAfterDelay(5f));
    }

    // Update the car's appearance based on repair progress
    private void UpdateCarVisuals()
    {
        if (carRenderer == null) return;

        float repairProgress = (float)CarRepairMiniGame.MinigameCompletionCount / totalPartsNeeded;

        if (repairProgress >= 1.0f)
        {
            // Car is fully repaired
            if (fixedCarSprite != null)
            {
                carRenderer.sprite = fixedCarSprite;
            }
        }
        else
        {
            // Car is still broken
            if (brokenCarSprite != null)
            {
                carRenderer.sprite = brokenCarSprite;
            }
        }
    }

    // Update the UI to show current parts count
    private void UpdatePartsCountUI()
    {
        if (partsCountText != null)
        {
            partsCountText.text = $"Car Parts: {partsCollected}/{totalPartsNeeded}";
        }
    }

    // Show a temporary message
    private IEnumerator ShowTempMessage(GameObject messageObj, float duration)
    {
        if (messageObj == null) yield break;

        messageObj.SetActive(true);
        yield return new WaitForSeconds(duration);
        messageObj.SetActive(false);
    }

    // End the game after a delay
    private IEnumerator EndGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Load victory scene (adjust scene index as needed)
        SceneManager.LoadScene("VictoryScene");
    }

    // Save car parts data to PlayerPrefs
    private void SaveCarPartsData()
    {
        PlayerPrefs.SetInt("CarPartsCollected", partsCollected);
        PlayerPrefs.SetInt("CarRepairCount", CarRepairMiniGame.MinigameCompletionCount);
        PlayerPrefs.Save();
    }

    // Load car parts data from PlayerPrefs
    private void LoadCarPartsData()
    {
        if (PlayerPrefs.HasKey("CarPartsCollected"))
        {
            partsCollected = PlayerPrefs.GetInt("CarPartsCollected");
        }
    }
}