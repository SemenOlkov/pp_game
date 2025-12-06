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
    
    private static FlashLightManager instance;
    private Coroutine batteryDrainCoroutine;
    private bool isDraining = false;
    
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

    // void Awake()
    // {
    //     if (instance != null && instance != this)
    //     {
    //         Destroy(gameObject);
    //         return;
    //     }
    //     instance = this;
    // }

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
        
        UpdateBatteryUI();
    }

    void Update()
    {
        // Автоматическое выключение при разрядке
        if (flashlight != null && flashlight.activeSelf && currentBattery <= 0)
        {
            TurnOffFlashlight();
            ShowMessage("Батарея разряжена. Необходимо поменять батарейки!");
        }
    }

    // Публичные методы для управления фонариком из других скриптов
    public void ToggleFlashlight()
    {
        if (flashlight == null) return;
        
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
        while (isDraining && currentBattery > 0)
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
        if (currentBattery <= 0)
        {
            TurnOffFlashlight();
        }
    }

    // Методы для управления батареей
    public void RechargeBattery(float amount)
    {
        currentBattery = Mathf.Min(100, currentBattery + amount);
        UpdateBatteryUI();
        Debug.Log($"Батарея заряжена до {currentBattery:F1}%");
    }

    public void ReplaceBattery()
    {
        currentBattery = 100f;
        UpdateBatteryUI();
        Debug.Log("Батарея заменена");
    }

    public void SetBattery(float value)
    {
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

    // UI методы
    private void UpdateBatteryUI()
    {
        if (batteryText != null)
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
}