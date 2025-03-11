using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private string spawnPointID;

    // Property to access the ID from other scripts
    public string SpawnPointID => spawnPointID;

    private void Start()
    {
        Debug.Log($"[PlayerSpawner] Initialized: {gameObject.name}, ID: {spawnPointID}, Position: {transform.position}");
    }

    // Visual gizmo to see spawn points in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f); // Changed to wire sphere for 2D visibility
        Vector3 forward2D = new Vector3(transform.right.x, transform.right.y, 0); // Use right vector for 2D forward
        Gizmos.DrawRay(transform.position, forward2D * 2);

        // Draw text in scene view - only works in editor
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up, $"ID: {spawnPointID}");
#endif
    }
}