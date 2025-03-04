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
    }
}