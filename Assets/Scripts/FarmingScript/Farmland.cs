//using UnityEngine;

//public class Farmland : MonoBehaviour
//{
//    private SpriteRenderer spriteRenderer;
//    private CropData currentCropData;
//    private int growthStage = 0; // Giai đoạn phát triển
//    //public GameObject cropPrefab; // Thêm prefab cây trồng vào farmland

//    private void Awake()
//    {
//        spriteRenderer = GetComponent<SpriteRenderer>(); // Lấy SpriteRenderer của farmland
//    }

//    public void PlantCrop(CropData cropData)
//    {
//        if (cropData == null || cropData.growthStages.Length == 0)
//        {
//            Debug.LogError("⚠️ CropData không hợp lệ hoặc thiếu sprite!");
//            return;
//        }

//        // Tạo GameObject mới để làm cây trồng
//        GameObject newCrop = new GameObject("Crop");
//        newCrop.transform.position = transform.position;
//        newCrop.transform.SetParent(transform);

//        // Thêm SpriteRenderer để hiển thị hình ảnh cây trồng
//        SpriteRenderer spriteRenderer = newCrop.AddComponent<SpriteRenderer>();
//        spriteRenderer.sprite = cropData.growthStages[0]; // Sprite trạng thái đầu tiên
//        spriteRenderer.sortingLayerName = "WalkInFront"; // Đảm bảo cây trồng hiển thị đúng layer

//        // Gán CropData vào đối tượng cây trồng
//        Crop cropComponent = newCrop.AddComponent<Crop>();
//        cropComponent.cropData = cropData;
//    }


//    private void GrowCrop()
//    {
//        if (currentCropData == null) return;

//        growthStage++;
//        if (growthStage < currentCropData.growthStages.Length)
//        {
//            spriteRenderer.sprite = currentCropData.growthStages[growthStage];
//            Debug.Log($"{currentCropData.cropName} phát triển lên giai đoạn {growthStage}");
//        }
//        else
//        {
//            CancelInvoke(nameof(GrowCrop)); // Dừng phát triển khi chín
//            Debug.Log($"{currentCropData.cropName} đã chín!");
//        }
//    }
//}

using UnityEngine;

public class Farmland : MonoBehaviour
{
    AudioManager audioManager;
    private Crop currentCrop; // Cây trồng hiện tại

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    public void PlantCrop(CropData cropData)
    {
        if (cropData == null || cropData.growthStages.Length == 0)
        {
            Debug.LogError("⚠️ CropData không hợp lệ hoặc thiếu sprite!");
            return;
        }

        // Xóa cây cũ (nếu có)
        if (currentCrop != null) Destroy(currentCrop.gameObject);

        // Tạo GameObject mới để làm cây trồng
        audioManager.PlaySFX(audioManager.crops);
        GameObject newCrop = new GameObject(cropData.cropName);
        newCrop.transform.position = transform.position;
        newCrop.transform.SetParent(transform);

        // Thêm Component Crop để xử lý logic phát triển
        currentCrop = newCrop.AddComponent<Crop>();
        currentCrop.Init(cropData);

        // ✅ Đảm bảo cây trồng có Sorting Layer "WalkBehind"
        SpriteRenderer sr = newCrop.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "WalkBehind";
            sr.sortingOrder = 5;
        }
    }

    public void ClearCrop()
    {
        if (currentCrop != null)
        {
            Destroy(currentCrop.gameObject);
            currentCrop = null;
        }
    }
}


