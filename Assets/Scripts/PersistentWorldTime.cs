using UnityEngine;

public class PersistentWorldTime : MonoBehaviour
{
    private static PersistentWorldTime instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}