using UnityEngine;
using System.IO;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public Inventory inventory;
    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            inventory = new Inventory();
            savePath = Application.persistentDataPath + "/inventory.json";
            LoadInventory();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveInventory()
    {
        string json = JsonUtility.ToJson(new InventorySaveData(inventory));
        File.WriteAllText(savePath, json);
    }

    public void LoadInventory()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);
            inventory.LoadFromSaveData(data);
        }
    }
}
