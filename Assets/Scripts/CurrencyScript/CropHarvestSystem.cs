using UnityEngine;

// This script should be attached to each Crop game object or added during crop creation
public class CropHarvestSystem : MonoBehaviour
{
    private Crop cropComponent;
    private string cropName;
    private int harvestCount = 0;
    private int maxHarvests = 1;
    private bool isFullyGrown = false;
    private bool canHarvest = false;

    // UI components
    [SerializeField] private GameObject harvestPrompt;

    // Visuals
    [SerializeField] private ParticleSystem harvestParticles;

    // Player detection
    private bool playerInRange = false;

    private void Awake()
    {
        // Get the Crop component
        cropComponent = GetComponent<Crop>();

        if (cropComponent != null && cropComponent.cropData != null)
        {
            cropName = cropComponent.cropData.cropName;

            // Get max harvests from CropValueManager
            if (CropValueManager.Instance != null)
            {
                maxHarvests = CropValueManager.Instance.GetGrowthCycles(cropName);
            }
        }
        else
        {
            Debug.LogError("CropHarvestSystem: Missing Crop component or CropData!");
        }

        // Hide harvest prompt initially
        if (harvestPrompt != null)
        {
            harvestPrompt.SetActive(false);
        }
    }

    // Called by the Crop component when fully grown
    public void OnCropFullyGrown()
    {
        isFullyGrown = true;
        canHarvest = true;

        // Show harvest prompt if player is in range
        if (playerInRange && harvestPrompt != null)
        {
            harvestPrompt.SetActive(true);
        }
    }

    private void Update()
    {
        // Check for harvest input
        if (playerInRange && canHarvest && Input.GetKeyDown(KeyCode.E))
        {
            HarvestCrop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // Show harvest prompt if crop is ready
            if (canHarvest && harvestPrompt != null)
            {
                harvestPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // Hide harvest prompt
            if (harvestPrompt != null)
            {
                harvestPrompt.SetActive(false);
            }
        }
    }

    private void HarvestCrop()
    {
        if (!canHarvest) return;

        // Get crop value from manager
        if (CropValueManager.Instance != null)
        {
            // Sell the crop at current position
            CropValueManager.Instance.SellCrop(cropName, transform.position);
        }

        // Play harvest effect
        if (harvestParticles != null)
        {
            harvestParticles.Play();
        }

        // Play sound
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.crops);
        }

        // Increment harvest count
        harvestCount++;

        // Check if we've reached max harvests
        if (harvestCount >= maxHarvests)
        {
            // Destroy the crop after last harvest
            Destroy(gameObject);
        }
        else
        {
            // Reset for next harvest cycle
            canHarvest = false;

            // Hide prompt
            if (harvestPrompt != null)
            {
                harvestPrompt.SetActive(false);
            }

            // Reset visuals to show the crop is regrowing
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (renderer != null && cropComponent != null && cropComponent.cropData.growthStages.Length > 0)
            {
                // Set to middle growth stage
                int midStage = Mathf.Max(0, cropComponent.cropData.growthStages.Length / 2 - 1);
                renderer.sprite = cropComponent.cropData.growthStages[midStage];
            }

            // Start regrowth timer
            StartRegrowthTimer();
        }
    }

    private void StartRegrowthTimer()
    {
        // Get regrowth time from crop data or use default
        float regrowthTime = 60f; // Default: 1 minute
        if (cropComponent != null && cropComponent.cropData != null)
        {
            // Assuming cropData has a regrowthTime field or use half of normal growth time
            regrowthTime = cropComponent.cropData.timeToGrow / 2f;
        }

        // Start coroutine to wait for regrowth
        StartCoroutine(RegrowthCoroutine(regrowthTime));
    }

    private System.Collections.IEnumerator RegrowthCoroutine(float regrowthTime)
    {
        yield return new WaitForSeconds(regrowthTime);

        // Crop is ready to harvest again
        canHarvest = true;

        // Update visuals to show it's harvestable
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null && cropComponent != null && cropComponent.cropData.growthStages.Length > 0)
        {
            // Set to final growth stage
            int finalStage = cropComponent.cropData.growthStages.Length - 1;
            renderer.sprite = cropComponent.cropData.growthStages[finalStage];
        }

        // Show prompt if player is in range
        if (playerInRange && harvestPrompt != null)
        {
            harvestPrompt.SetActive(true);
        }
    }
}