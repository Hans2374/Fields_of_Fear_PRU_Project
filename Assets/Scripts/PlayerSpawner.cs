using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Transform defaultSpawnPoint;

    private void Start()
    {
        // Get player reference - it should exist because of DontDestroyOnLoad
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Position player based on transition data
            if (GameManager.instance != null && GameManager.instance.IsComingFromTransition())
            {
                player.transform.position = GameManager.instance.GetPlayerSpawnPoint();
            }
            else if (defaultSpawnPoint != null)
            {
                player.transform.position = defaultSpawnPoint.position;
            }
        }
        else
        {
            Debug.LogError("Player not found. Make sure it has the 'Player' tag and PersistentPlayer component.");
        }

        if (player != null)
        {
            if (GameManager.instance != null && GameManager.instance.IsComingFromTransition())
            {
                Vector2 spawnPoint = GameManager.instance.GetPlayerSpawnPoint();
                Debug.Log("Positioning player at: " + spawnPoint); // Add this line
                player.transform.position = spawnPoint;
            }
            else if (defaultSpawnPoint != null)
            {
                Debug.Log("Using default spawn at: " + defaultSpawnPoint.position); // Add this line
                player.transform.position = defaultSpawnPoint.position;
            }
        }
    }
}