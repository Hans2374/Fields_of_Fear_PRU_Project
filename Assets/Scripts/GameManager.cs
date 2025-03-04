using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private Vector2 playerSpawnPoint;
    private bool isComingFromTransition = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerSpawnPoint(Vector2 spawnPoint)
    {
        playerSpawnPoint = spawnPoint;
        isComingFromTransition = true;
    }

    public Vector2 GetPlayerSpawnPoint()
    {
        return playerSpawnPoint;
    }

    public bool IsComingFromTransition()
    {
        bool result = isComingFromTransition;
        isComingFromTransition = false;  // Reset after reading
        return result;
    }
}