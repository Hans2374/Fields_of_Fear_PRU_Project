using UnityEngine;

public class SceneMove : MonoBehaviour
{
    [SerializeField] private string destinationAreaName; // "Main" or "Shop"
    [SerializeField] private string spawnPointID;        // The ID of the spawn point to use in the destination area

    // Reference to the transition manager
    private TransitionManager transitionManager;

    private void Start()
    {
        // Find the TransitionManager in the scene
        transitionManager = FindObjectOfType<TransitionManager>();

        if (transitionManager == null)
        {
            Debug.LogError("[SceneMove] TransitionManager not found in the scene! Make sure it exists.");
        }
        else
        {
            Debug.Log($"[SceneMove] {gameObject.name} initialized. Destination: {destinationAreaName}, SpawnPoint: {spawnPointID}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[SceneMove] Trigger entered by: {other.name} with tag: {other.tag}");

        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[SceneMove] Player detected, attempting transition to {destinationAreaName} at spawn point {spawnPointID}");

            // Call the TransitionManager to handle the transition
            if (transitionManager != null)
            {
                // This assumes your TransitionManager has a method to handle area transitions
                transitionManager.TransitionToArea(destinationAreaName, spawnPointID);
            }
            else
            {
                Debug.LogError("[SceneMove] Cannot transition: TransitionManager reference is missing!");
            }
        }
    }

    // Add visual gizmo for easy identification in editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Use BoxCollider2D bounds if available, otherwise use a default size
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            // For 2D box colliders
            Vector3 size = new Vector3(boxCollider.size.x, boxCollider.size.y, 0.1f);
            Vector3 center = boxCollider.offset;
            center.z = 0;
            Gizmos.DrawWireCube(transform.position + center, size);
        }
        else
        {
            // Fallback if no BoxCollider2D is found
            Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 0.1f));
        }

        // Draw text in scene view - only works in editor
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2,
            $"To: {destinationAreaName}\nSpawn: {spawnPointID}");
#endif
    }
}