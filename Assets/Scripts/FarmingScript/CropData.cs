using UnityEngine;

[CreateAssetMenu(fileName = "New Crop", menuName = "Farming/CropData")]
public class CropData : ScriptableObject
{
    

    public string cropName;
    public Sprite[] growthStages;
    public float timeToGrow;
    public int sellPrice;
}
