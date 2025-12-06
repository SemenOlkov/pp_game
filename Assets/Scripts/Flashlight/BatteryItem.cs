using UnityEngine;

[CreateAssetMenu(fileName = "Battery Item", menuName = "Inventory/Items/BatteryItem")]
public class BatteryItem : ItemScriptableObject
{
    [Range(0, 100)]
    public float chargeAmount = 100f;
    
    private void Start()
    {
        itemType = ItemType.Battery;
        isConsumable = true;
    }

    public override void Functionality()
    {
        if (FlashLightManager.Instance != null)
        {
            FlashLightManager.Instance.RechargeBattery(chargeAmount);
            Debug.Log($"Использована батарейка. Заряд: {chargeAmount}%");
        }
    }
}