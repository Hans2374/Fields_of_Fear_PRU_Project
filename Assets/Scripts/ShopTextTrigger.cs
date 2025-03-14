using UnityEngine;
using TMPro;

public class ShopTextTrigger : MonoBehaviour
{
    [Header("Text to Display")]
    [SerializeField] private GameObject textContainer; // Parent container for the TMP element
    [SerializeField] private TextMeshProUGUI promptText; // Reference to the TMP component
    [SerializeField] private string promptMessage = ""; // Default message

    [Header("Trigger Settings")]
    [SerializeField] private float displayDistance = 2f; // Distance to display the text
    [SerializeField] private bool useColliderTrigger = true; // Use OnTrigger events or distance check
    [SerializeField] private LayerMask playerLayer; // Layer for the player (if using distance check)

    [Header("Debug")]
    [SerializeField] private bool debugMode = false; // Enable debug logs

    // Internal state
    private bool isPlayerInRange = false;
    private Transform playerTransform;

    private void Start()
    {
        // Initially hide the text
        if (textContainer != null)
        {
            textContainer.SetActive(false);
        }
        else if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError($"[ShopTextTrigger] No text elements assigned on {gameObject.name}!");
        }

        // Find player if not using collider triggers
        if (!useColliderTrigger)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (playerTransform == null)
            {
                Debug.LogError("[ShopTextTrigger] Player not found! Make sure it has the 'Player' tag.");
            }
        }

        // Set up text if needed
        if (promptText != null && !string.IsNullOrEmpty(promptMessage))
        {
            promptText.text = promptMessage;
        }
    }

    private void Update()
    {
        // If not using trigger colliders, check distance to player
        if (!useColliderTrigger && playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= displayDistance && !isPlayerInRange)
            {
                // Player just entered range
                isPlayerInRange = true;
                ShowText();
                if (debugMode) Debug.Log($"[ShopTextTrigger] Player entered range of {gameObject.name}");
            }
            else if (distanceToPlayer > displayDistance && isPlayerInRange)
            {
                // Player just left range
                isPlayerInRange = false;
                HideText();
                if (debugMode) Debug.Log($"[ShopTextTrigger] Player left range of {gameObject.name}");
            }
        }
    }

    // Handle trigger enter/exit for collider-based approach
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!useColliderTrigger) return;

        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            ShowText();
            if (debugMode) Debug.Log($"[ShopTextTrigger] Player entered trigger of {gameObject.name}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!useColliderTrigger) return;

        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            HideText();
            if (debugMode) Debug.Log($"[ShopTextTrigger] Player left trigger of {gameObject.name}");
        }
    }

    // Show the text UI
    private void ShowText()
    {
        if (textContainer != null)
        {
            textContainer.SetActive(true);
        }
        else if (promptText != null)
        {
            promptText.gameObject.SetActive(true);
        }
    }

    // Hide the text UI
    private void HideText()
    {
        if (textContainer != null)
        {
            textContainer.SetActive(false);
        }
        else if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    // Visual debugging
    private void OnDrawGizmosSelected()
    {
        if (!useColliderTrigger)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, displayDistance);
        }
    }
}