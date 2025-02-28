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
}
