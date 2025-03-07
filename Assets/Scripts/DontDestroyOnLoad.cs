using UnityEngine;
using UnityEngine.SceneManagement;
// Nếu bạn xài TextMeshPro, cần thêm:
using TMPro;
// Nếu bạn xài UI Image, cần thêm:
using UnityEngine.UI;

public class DontDestroyOnLoad : MonoBehaviour
{
    public static DontDestroyOnLoad Instance;

    [Header("Canvas Gốc")]
    public Canvas mainCanvas;

    [Header("Stamina Bar")]
    // Tham chiếu tới GameObject 'Stamina Bar' nếu muốn
    public GameObject staminaBarRoot;
    // Hoặc nếu bạn cần điều khiển hình ảnh thanh stamina
    public Image staminaFill;
    public Image staminaBackground;
    // Tuỳ theo bạn có đối tượng BGR, Stamina,... bạn kéo vào

    [Header("World Time")]
    public GameObject worldTimeRoot;
    // Giả sử bạn có text hiển thị thời gian
    public TMP_Text timeText; // hoặc Text nếu chưa dùng TMP

    [Header("UI_Inventory")]
    public GameObject inventoryRoot;
    public GameObject background1;
    public GameObject background2;
    public Transform itemSlotContainer;
    // Hoặc thêm Image, Text, script Inventory,... tuỳ bạn

    [Header("Setting")]
    public TMP_Text settingText;
    public GameObject settingMenu;
    public GameObject panel;
    public GameObject cancelButton;
    // v.v...

    [Header("Life")]
    public Image heart1;
    public Image heart2;

    [Header("Systems")]
    public GameObject systemsRoot;
    // Hoặc script Systems nếu bạn có

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Nếu đã có Instance khác tồn tại, huỷ bớt để tránh trùng lặp
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Nếu cần làm gì khi Scene mới load xong, bạn có thể xử lý ở đây
        Debug.Log("Scene loaded: " + scene.name);
    }
}
