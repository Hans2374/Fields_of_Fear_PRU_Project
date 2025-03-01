using UnityEngine;
using static Item;

public class ItemWorldSpawner : MonoBehaviour
{
    public Item item;
    private void Awake()
    {
        ItemWorld.SpawnItemWorld(transform.position, item);
        Destroy(gameObject);

    }
}
