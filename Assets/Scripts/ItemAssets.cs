using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Transform pfItemWorld;

    // Đảm bảo các biến này tồn tại và viết đúng tên
    public Sprite BerrySeedSprite;
    public Sprite CarrotSeedSprite;
    public Sprite GrapeSeedSprite;
    public Sprite PotatoSeedSprite;
    public Sprite RadishSeedSprite;
    public Sprite CabbageSeedSprite;
    public Sprite TomatoSeedSprite;
    public Sprite CarPartSprite;

    // Add this method to ItemAssets.cs to verify all required sprites are assigned

    public void VerifyAllSpritesAssigned()
    {
        bool allValid = true;

        // Check car part sprite
        if (CarPartSprite == null)
        {
            Debug.LogError("CarPartSprite is not assigned in ItemAssets!");
            allValid = false;
        }

        // Check seed sprites
        if (BerrySeedSprite == null)
        {
            Debug.LogError("BerrySeedSprite is not assigned in ItemAssets!");
            allValid = false;
        }

        if (CarrotSeedSprite == null)
        {
            Debug.LogError("CarrotSeedSprite is not assigned in ItemAssets!");
            allValid = false;
        }

        if (GrapeSeedSprite == null)
        {
            Debug.LogError("GrapeSeedSprite is not assigned in ItemAssets!");
            allValid = false;
        }

        if (PotatoSeedSprite == null)
        {
            Debug.LogError("PotatoSeedSprite is not assigned in ItemAssets!");
            allValid = false;
        }

        if (RadishSeedSprite == null)
        {
            Debug.LogError("RadishSeedSprite is not assigned in ItemAssets!");
            allValid = false;
        }

        if (CabbageSeedSprite == null)
        {
            Debug.LogError("CabbageSeedSprite is not assigned in ItemAssets!");
            allValid = false;
        }

        if (TomatoSeedSprite == null)
        {
            Debug.LogError("TomatoSeedSprite is not assigned in ItemAssets!");
            allValid = false;
        }

        if (pfItemWorld == null)
        {
            Debug.LogError("pfItemWorld prefab is not assigned in ItemAssets!");
            allValid = false;
        }

        if (allValid)
        {
            Debug.Log("All required sprites and prefabs are properly assigned in ItemAssets!");
        }
        else
        {
            Debug.LogError("Some required sprites or prefabs are missing in ItemAssets! Check the console for details.");
        }
    }

    // Call this from Start() method:
    private void Start()
    {
        // Verify all sprites are assigned
        VerifyAllSpritesAssigned();
    }
}
