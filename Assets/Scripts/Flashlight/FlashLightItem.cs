using UnityEngine;

[CreateAssetMenu(fileName = "Flashlight Item", menuName = "Inventory/Items/FlashLightItem")]
public class FlashlightItem : ItemScriptableObject
{
    [Header("Battery Settings")]
    public float batteryRechargeAmount = 100f;
    public bool canRecharge = true;
    
    private void Start()
    {
        itemType = ItemType.Flashlight;
    }

    public override void Functionality()
    {
        if (FlashLightManager.Instance == null)
        {
            Debug.LogError("FlashLightManager не найден в сцене!");
            return;
        }
        
        // Проверяем, включен ли фонарик
        if (FlashLightManager.Instance.IsFlashlightOn())
        {
            // Если включен - выключаем
            FlashLightManager.Instance.TurnOffFlashlight();
        }
        else
        {
            // Если выключен - пытаемся включить
            // Менеджер сам проверит заряд батареи
            FlashLightManager.Instance.TurnOnFlashlight();
        }
    }
    
    // Дополнительный метод для замены батареек
    public void ReplaceBattery()
    {
        if (FlashLightManager.Instance != null)
        {
            FlashLightManager.Instance.ReplaceBattery();
        }
    }
    
    // Метод для зарядки (если нужна постепенная зарядка)
    public void RechargeBattery()
    {
        if (FlashLightManager.Instance != null && canRecharge)
        {
            FlashLightManager.Instance.RechargeBattery(batteryRechargeAmount);
        }
    }
    
    public bool CanBeUsed()
    {
        return FlashLightManager.Instance != null && FlashLightManager.Instance.HasBattery();
    }
    
    public float GetCurrentBattery()
    {
        return FlashLightManager.Instance != null ? FlashLightManager.Instance.GetBatteryPercentage() : 0f;
    }
}