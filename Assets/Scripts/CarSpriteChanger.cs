using UnityEngine;

public class CarSpriteChanger : MonoBehaviour
{
    public Sprite newSprite; // Sprite mới khi đủ 5 lần hoàn thành
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (CarRepairMiniGame.MinigameCompletionCount >= 5)
        {
            ChangeCarSprite();
        }
    }

    private void ChangeCarSprite()
    {
        if (spriteRenderer != null && newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
            Debug.Log("Đã đổi sprite của xe!");
        }
    }
}
