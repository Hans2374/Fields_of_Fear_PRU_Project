using System.Collections;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public ItemType itemType; // Loại item
    public float moveSpeed = 5f; // Tốc độ bay về phía Player
    private Transform target; // Vị trí của Player

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"[DEBUG] {gameObject.name} chạm vào {collision.gameObject.name}");
            target = collision.transform; // Lưu vị trí Player
            StartCoroutine(MoveToPlayer());
        }
    }

    private IEnumerator MoveToPlayer()
    {
        while (target != null && Vector2.Distance(transform.position, target.position) > 0.1f)
        {
            transform.position = Vector2.Lerp(transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return null; // Chờ frame tiếp theo
        }

        if (target != null)
        {
            Inventory.Instance.AddItem(itemType);
            Debug.Log($"✅ Nhặt thành công: {itemType}");
            Destroy(gameObject);
        }
    }
}
