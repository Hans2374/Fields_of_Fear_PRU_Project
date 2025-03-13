using UnityEngine;

public class PersistentAudio : MonoBehaviour
{
    private static PersistentAudio instance;

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