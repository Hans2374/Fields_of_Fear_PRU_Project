using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingOrderHandler : MonoBehaviour
{
    public GameObject player;
    public int sortingOrderOffset = 0;  // Tùy chỉnh nếu bạn cần một giá trị offset
    private SpriteRenderer spriteRenderer;
    private Collider2D[] colliders;  // Mảng để chứa các Collider2D

    void Start()
    {
        // Lấy component SpriteRenderer từ đối tượng mà script này gắn vào
        spriteRenderer = GetComponent<SpriteRenderer>();

        player = GameObject.FindWithTag("Player");

        // Lấy tất cả các Collider2D trên đối tượng này
        colliders = GetComponents<Collider2D>();
    }

    void Update()
    {
        if (spriteRenderer != null && colliders.Length > 0)
        {
            // Tìm giá trị Y cao nhất từ các collider
            float highestColliderTopY = float.MinValue;

            foreach (Collider2D collider in colliders)
            {
                // Tìm giá trị Y của cạnh trên của từng collider
                float colliderTopY = collider.bounds.max.y;

                // Cập nhật giá trị Y cao nhất
                if (colliderTopY > highestColliderTopY)
                {
                    highestColliderTopY = colliderTopY;
                }
            }

            // So sánh vị trí Y của player với cạnh trên của collider cao nhất
            if (player.transform.position.y < highestColliderTopY)
            {
                // Nếu nhân vật nằm phía dưới cạnh trên của collider, nhân vật sẽ nằm trước vật
                spriteRenderer.sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder - 1 + sortingOrderOffset;
            }
            else
            {
                // Nếu nhân vật nằm phía trên cạnh trên của collider, nhân vật sẽ nằm sau vật
                spriteRenderer.sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder + 1 + sortingOrderOffset;
            }
        }
    }
}
