using UnityEngine;

// Place this script on the same object as your TransitionManager
public class DoorbellHelper : MonoBehaviour
{
    public static void PlayDoorbell()
    {
        AudioManager audioManager = FindAudioManager();
        if (audioManager != null && audioManager.bellDoor != null)
        {
            Debug.Log("[DoorbellHelper] Playing doorbell sound");
            audioManager.PlaySFX(audioManager.bellDoor);
        }
        else
        {
            Debug.LogError("[DoorbellHelper] Cannot play doorbell - check AudioManager and bellDoor clip");
        }
    }

    private static AudioManager FindAudioManager()
    {
        // Method 1: FindObjectOfType
        AudioManager audioManager = GameObject.FindObjectOfType<AudioManager>();
        if (audioManager != null) return audioManager;

        // Method 2: GameObject.FindWithTag
        GameObject audioObj = GameObject.FindWithTag("Audio");
        if (audioObj != null)
        {
            audioManager = audioObj.GetComponent<AudioManager>();
            if (audioManager != null) return audioManager;
        }

        // Method 3: Search all active game objects (expensive, but thorough)
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            AudioManager am = obj.GetComponent<AudioManager>();
            if (am != null) return am;
        }

        return null;
    }
}