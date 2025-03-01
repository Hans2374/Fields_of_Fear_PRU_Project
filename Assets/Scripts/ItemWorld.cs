using TMPro;
using UnityEditorInternal;
using UnityEngine;
using CodeMonkey.Utils;
public class ItemWorld : MonoBehaviour
{
    public static ItemWorld SpawnItemWorld(Vector3 position, Item item)
    {
        if (ItemAssets.Instance == null)
        {
            Debug.LogError("ItemAssets.Instance chưa được khởi tạo!");
            return null;
        }
        if (ItemAssets.Instance.pfItemWorld == null)
        {
            Debug.LogError("pfItemWorld chưa được gán trong ItemAssets!");
            return null;
        }

        Transform transform = Instantiate(ItemAssets.Instance.pfItemWorld, position, Quaternion.identity);
        ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
        itemWorld.SetItem(item);
        return itemWorld;
    }

    public static ItemWorld DropItem(Vector3 dropPosition, Item item)
    {
            Vector3 randomDir = UtilsClass.GetRandomDir();
        ItemWorld itemWorld = SpawnItemWorld(dropPosition + randomDir * 10f, item);
        itemWorld.GetComponent<Rigidbody2D>().AddForce(randomDir * 10f, ForceMode2D.Impulse);
        return itemWorld;
    }

    private Item item;

    private SpriteRenderer spriteRenderer;
    private TextMeshPro textMeshPro;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
    }
    public void SetItem(Item item)
    {
        this.item = item;
        spriteRenderer.sprite = item.GetSprite();
        if (item.amount > 1)
        {
            textMeshPro.SetText(item.amount.ToString());
        }
        else
        {
            textMeshPro.SetText("");
        }
    }

    public Item GetItem()
    {
        return item;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
