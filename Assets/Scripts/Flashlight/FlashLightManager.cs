using UnityEngine;
using TMPro;
using System.Collections;

public class FlashLightManager : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public GameObject flashlight;
    public TextMeshProUGUI batteryText;
    
    [Header("Battery Settings")]
    [Range(0, 100)]
    public float currentBattery = 100f;
    public float batteryDrainRate = 1f; // % в секунду
    public float lowBatteryThreshold = 20f;
    public Color normalBatteryColor = Color.white;
    public Color lowBatteryColor = Color.yellow;
    public Color criticalBatteryColor = Color.red;
    
    [Header("Messages")]
    public TextMeshProUGUI messageText;
    public float messageDisplayTime = 2f;
    
    [Header("Inventory Reference")]
    public InventoryManager inventoryManager;
    
    private static FlashLightManager instance;
    private Coroutine batteryDrainCoroutine;
    private bool isDraining = false;
    private bool hasFlashlightInInventory = false;
    
    public static FlashLightManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FlashLightManager>();
            }
            return instance;
        }
    }

    void Start()
    {
        if (flashlight != null)
        {
            flashlight.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Flashlight object не назначен в инспекторе!");
        }
        
        // Ищем InventoryManager, если не присвоен в инспекторе
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }
        
        UpdateFlashlightStatus();
        UpdateBatteryUI();
    }

    void Update()
    {
        // Проверяем наличие фонарика в инвентаре
        bool hadFlashlight = hasFlashlightInInventory;
        hasFlashlightInInventory = CheckFlashlightInInventory();
        if (flashlight != null && flashlight.activeSelf && !isDraining && hasFlashlightInInventory)
        {
            flashlight.SetActive(false);
        }
        
        // Если фонарик только что исчез из инвентаря
        if (hadFlashlight && !hasFlashlightInInventory)
        {
            // Выключаем фонарик, если он был включен
            if (flashlight != null && flashlight.activeSelf)
            {
                flashlight.SetActive(false);
                StopBatteryDrain();
                Debug.Log("Flashlight выброшен из инвентаря, свет выключен");
            }
            
            // Скрываем UI батареи
            UpdateBatteryUI();
        }
        // Если фонарик только что появился в инвентаре
        else if (!hadFlashlight && hasFlashlightInInventory)
        {
            // Показываем UI батареи
            UpdateBatteryUI();
        }
        
        // Автоматическое выключение при разрядке (только если фонарик есть в инвентаре)
        if (flashlight != null && flashlight.activeSelf && currentBattery <= 0 && hasFlashlightInInventory)
        {
            TurnOffFlashlight();
            ShowMessage("Батарея разряжена. Необходимо поменять батарейки!");
        }
    }

    public void RefreshFlashlightReference()
    {
        // Ищем объект с компонентом flashpass (ваша метка на фонарике)
        // Предполагаем, что flashpass висит на самом объекте света или его родителе
        FlashPass marker = FindObjectOfType<FlashPass>();
    
        if (marker != null)
        {
            flashlight = marker.gameObject;
            Debug.Log("[FlashLightManager] Ссылка на фонарик обновлена.");
        }
        else
        {
            Debug.LogWarning("[FlashLightManager] Объект с компонентом flashpass не найден!");
        }

        // Также обновляем ссылку на инвентарь, так как UI пересоздался
        inventoryManager = InventoryManager.Instance;
    }
    public void RegisterFlashlight(GameObject flashlightObject)
    {
        flashlight = flashlightObject;
        Debug.Log("[FlashLightManager] Фонарик успешно зарегистрирован через flashpass!");
    
        // Сразу проверяем инвентарь, чтобы включить/выключить UI батареи
        CheckInventoryForFlashlight();
    }

    // Публичные методы для управления фонариком из других скриптов
    public void ToggleFlashlight()
    {
        if (flashlight == null) return;
        
        // Проверяем, есть ли фонарик в инвентаре
        if (!hasFlashlightInInventory)
        {
            ShowMessage("У вас нет фонарика в инвентаре!");
            return;
        }
        
        if (flashlight.activeSelf)
        {
            TurnOffFlashlight();
        }
        else
        {
            TurnOnFlashlight();
        }
    }

    public void TurnOnFlashlight()
    {
        if (flashlight == null) return;
        
        // Проверяем наличие фонарика в инвентаре
        if (!hasFlashlightInInventory)
        {
            ShowMessage("У вас нет фонарика в инвентаре!");
            return;
        }
        
        // Проверка заряда батареи
        if (currentBattery <= 0)
        {
            ShowMessage("Фонарик разряжен. Необходимо поменять батарейки!");
            return;
        }
        
        flashlight.SetActive(true);
        StartBatteryDrain();
        Debug.Log($"Flashlight ON (Battery: {currentBattery:F1}%)");
    }

    public void TurnOffFlashlight()
    {
        if (flashlight == null) return;
        
        flashlight.SetActive(false);
        StopBatteryDrain();
        Debug.Log($"Flashlight OFF (Battery: {currentBattery:F1}%)");
    }

    public void SetFlashlight(bool state)
    {
        // Проверяем наличие фонарика в инвентаре
        if (state && !hasFlashlightInInventory)
        {
            ShowMessage("У вас нет фонарика в инвентаре!");
            return;
        }
        
        if (state) TurnOnFlashlight();
        else TurnOffFlashlight();
    }

    public bool IsFlashlightOn()
    {
        return flashlight != null && flashlight.activeSelf;
    }

    // Система заряда батареи
    private void StartBatteryDrain()
    {
        if (isDraining) return;
        
        isDraining = true;
        batteryDrainCoroutine = StartCoroutine(DrainBattery());
    }

    private void StopBatteryDrain()
    {
        if (!isDraining) return;
        
        isDraining = false;
        if (batteryDrainCoroutine != null)
        {
            StopCoroutine(batteryDrainCoroutine);
            batteryDrainCoroutine = null;
        }
    }

    private IEnumerator DrainBattery()
    {
        while (isDraining && currentBattery > 0 && hasFlashlightInInventory)
        {
            yield return new WaitForSeconds(1f);
            
            currentBattery -= batteryDrainRate;
            currentBattery = Mathf.Max(0, currentBattery);
            
            UpdateBatteryUI();
            
            // Предупреждение о низком заряде
            if (currentBattery <= lowBatteryThreshold && currentBattery > 0)
            {
                ShowMessage($"Низкий заряд батареи: {currentBattery:F0}%");
            }
        }
        
        // Если батарея разрядилась, выключаем фонарик
        if (currentBattery <= 0 && hasFlashlightInInventory)
        {
            TurnOffFlashlight();
        }
    }

    // Метод для проверки наличия фонарика в инвентаре
    private bool CheckFlashlightInInventory()
    {
        if (inventoryManager == null) return false;
        
        // Проверяем основные слоты инвентаря
        foreach (InventorySlot slot in inventoryManager.slots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Flashlight)
            {
                return true;
            }
        }
        
        // Проверяем слоты горячей панели
        foreach (InventorySlot slot in inventoryManager.hotbarSlots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Flashlight)
            {
                return true;
            }
        }
        
        return false;
    }

    // Обновляем статус фонарика в зависимости от инвентаря
    private void UpdateFlashlightStatus()
    {
        hasFlashlightInInventory = CheckFlashlightInInventory();
        
        // Если фонарика нет в инвентаре, выключаем его
        if (!hasFlashlightInInventory && flashlight != null && flashlight.activeSelf)
        {
            flashlight.SetActive(false);
            StopBatteryDrain();
        }
    }

    // Методы для управления батареей
    public void RechargeBattery(float amount)
    {
        // Только если есть фонарик в инвентаре
        if (!hasFlashlightInInventory)
        {
            ShowMessage("У вас нет фонарика для зарядки!");
            return;
        }
        
        currentBattery = Mathf.Min(100, currentBattery + amount);
        UpdateBatteryUI();
        Debug.Log($"Батарея заряжена до {currentBattery:F1}%");
    }

    public void ReplaceBattery()
    {
        // Только если есть фонарик в инвентаре
        if (!hasFlashlightInInventory)
        {
            ShowMessage("У вас нет фонарика для замены батареи!");
            return;
        }
        
        currentBattery = 100f;
        UpdateBatteryUI();
        Debug.Log("Батарея заменена");
    }

    public void SetBattery(float value)
    {
        // Только если есть фонарик в инвентаре
        if (!hasFlashlightInInventory) return;
        
        currentBattery = Mathf.Clamp(value, 0, 100);
        UpdateBatteryUI();
    }

    public float GetBatteryPercentage()
    {
        return currentBattery;
    }

    public bool HasBattery()
    {
        return currentBattery > 0;
    }

    public bool HasFlashlightInInventory()
    {
        return hasFlashlightInInventory;
    }

    // UI методы
    private void UpdateBatteryUI()
    {
        if (batteryText != null)
        {
            // Показываем текст только если есть фонарик в инвентаре
            batteryText.gameObject.SetActive(hasFlashlightInInventory);
            
            if (hasFlashlightInInventory)
            {
                batteryText.text = $"{currentBattery:F0}%";
                
                // Изменение цвета в зависимости от уровня заряда
                if (currentBattery <= 10)
                    batteryText.color = criticalBatteryColor;
                else if (currentBattery <= lowBatteryThreshold)
                    batteryText.color = lowBatteryColor;
                else
                    batteryText.color = normalBatteryColor;
            }
        }
    }

    private void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);
            
            // Автоматическое скрытие сообщения
            StopAllCoroutines();
            StartCoroutine(HideMessageAfterDelay());
        }
        
        Debug.Log(message);
    }

    private IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDisplayTime);
        
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    // Метод для принудительной проверки инвентаря (можно вызывать при изменении инвентаря)
    public void CheckInventoryForFlashlight()
    {
        UpdateFlashlightStatus();
        UpdateBatteryUI();
    }

    // Для дебага в инспекторе
    [ContextMenu("Заменить батарею")]
    private void DebugReplaceBattery()
    {
        ReplaceBattery();
    }

    [ContextMenu("Разрядить на 50%")]
    private void DebugDrain50()
    {
        SetBattery(currentBattery - 50);
    }

    [ContextMenu("Проверить наличие фонарика")]
    private void DebugCheckFlashlight()
    {
        Debug.Log($"Фонарик в инвентаре: {CheckFlashlightInInventory()}");
    }
}