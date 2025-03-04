using UnityEngine;

public class EnemySpawnDirect : MonoBehaviour
{
    void Start()
    {
        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        GameObject enemy = new GameObject("Enemy"); 
        enemy.transform.position = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f)); 

        SpriteRenderer spriteRenderer = enemy.AddComponent<SpriteRenderer>(); 
        spriteRenderer.sprite = Resources.Load<Sprite>("enemySprite"); 

        enemy.AddComponent<BoxCollider2D>(); 
        enemy.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; 
    }
}
