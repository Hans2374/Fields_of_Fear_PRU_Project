using UnityEngine;

public class ending : MonoBehaviour
{
    public float scrollSpeed = 1000f; // Tốc độ cuộn
    private RectTransform rectTransform;
    private float width;
    private bool isLooping = true; // Biến kiểm soát loop

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.rect.width; // Lấy chiều rộng của ảnh
    }

    void Update()
{
    if (!isLooping) return; // Nếu dừng loop thì không làm gì

    // Di chuyển background sang phải
    rectTransform.anchoredPosition += Vector2.right * scrollSpeed * Time.deltaTime;

    // Khi ảnh ra khỏi màn hình bên phải, reset vị trí về bên trái ảnh còn lại
    if (rectTransform.anchoredPosition.x >= width)
    {
        rectTransform.anchoredPosition -= new Vector2(width * 2, 0);
    }
}

    // Hàm dừng loop background
    public void StopLoop()
    {
        isLooping = false;
    }
}
