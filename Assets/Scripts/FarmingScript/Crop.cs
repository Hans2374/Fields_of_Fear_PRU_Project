//using System.Collections;
//using UnityEngine;

//public class Crop : MonoBehaviour
//{
//    public CropData cropData;
//    private SpriteRenderer spriteRenderer;
//    private int currentStage = 0;
//    private float growthTime;

//    private void Start()
//    {
//        spriteRenderer = GetComponent<SpriteRenderer>();

//        if (cropData == null)
//        {
//            Debug.LogError("CropData chưa được gán!", this);
//            return;
//        }

//        // Kiểm tra và tính toán thời gian phát triển an toàn
//        int stages = cropData.growthStages.Length;
//        if (stages == 0)
//        {
//            Debug.LogError($"CropData {cropData.cropName} không có stage nào!", this);
//            return;
//        }

//        growthTime = (stages > 1) ? cropData.timeToGrow / (stages - 1) : cropData.timeToGrow;
//        SetCropData();
//        StartCoroutine(GrowCrop());
//    }

//    private void SetCropData()
//    {
//        if (cropData.growthStages.Length > currentStage)
//        {
//            spriteRenderer.sprite = cropData.growthStages[currentStage];
//            Debug.Log($"[{cropData.cropName}] Đang ở stage {currentStage}/{cropData.growthStages.Length - 1}");
//        }
//        else
//        {
//            Debug.LogError($"[{cropData.cropName}] Không tìm thấy sprite cho stage {currentStage}");
//        }
//    }

//    private IEnumerator GrowCrop()
//    {
//        while (currentStage < cropData.growthStages.Length - 1)
//        {
//            yield return new WaitForSeconds(growthTime);
//            currentStage++;
//            SetCropData();
//        }
//        Debug.Log($"[{cropData.cropName}] Đã trưởng thành!");
//    }
//}
using UnityEngine;
using System.Collections;

public class Crop : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public CropData cropData;
    private int growthStage = 0;
    private bool isFullyGrown = false;

    public void Init(CropData data)
    {
        cropData = data;
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = cropData.growthStages[0]; // Bắt đầu từ giai đoạn đầu
        spriteRenderer.sortingLayerName = "WalkBehind"; // Gán layer đúng như yêu cầu
        spriteRenderer.sortingOrder = 5; // Đảm bảo không bị che khuất

        // ✅ Thêm Collider2D để Raycast có thể phát hiện cây này
        BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }

        // Điều chỉnh BoxCollider2D
        collider.size *= 2.55f;  // Tăng kích thước BoxCollider lên 1.5 lần
        collider.isTrigger = true; // Cho phép collider hoạt động như trigger (không cản trở di chuyển)

        StartCoroutine(Grow());
    }

    // private IEnumerator Grow()
    // {
    //     while (growthStage < cropData.growthStages.Length - 1)
    //     {
    //         yield return new WaitForSeconds(cropData.timeToGrow);
    //         growthStage++;
    //         spriteRenderer.sprite = cropData.growthStages[growthStage];
    //         Debug.Log($"{cropData.cropName} đã phát triển đến giai đoạn {growthStage}");
    //     }


    //     Debug.Log($"{cropData.cropName} đã chín! 🌱");
    //     isFullyGrown = true;
    // }
    private IEnumerator Grow()
    {
        while (growthStage < cropData.growthStages.Length - 1)
        {
            yield return new WaitForSeconds(cropData.timeToGrow);
            growthStage++;
            spriteRenderer.sprite = cropData.growthStages[growthStage];
            Debug.Log($"{cropData.cropName} đã phát triển đến giai đoạn {growthStage}");
        }

        // Khi cây chín, tăng vị trí Y lên 0.2 đơn vị
        transform.position += new Vector3(0, 0.1f, 0);
        Debug.Log($"{cropData.cropName} đã chín! 🌱");
        isFullyGrown = true;

        Debug.Log($"{cropData.cropName} đã chín! 🌱");
        isFullyGrown = true;
    }


    private void OnMouseOver()
    {
        if (isFullyGrown && Input.GetMouseButtonDown(1))
        {
            HarvestCrop();
        }
    }

    private void HarvestCrop()
    {
        if (!isFullyGrown)
        {
            Debug.LogWarning("Cây chưa chín, không thể thu hoạch!");
            return;
        }

        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddMoney(cropData.sellPrice);
            Debug.Log($"Đã thu hoạch {cropData.cropName} và nhận được {cropData.sellPrice} tiền!");
        }
        else
        {
            Debug.LogWarning("CurrencyManager instance not found!");
        }
        Destroy(gameObject);
    }
}