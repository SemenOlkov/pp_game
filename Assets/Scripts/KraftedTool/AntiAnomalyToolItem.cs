using UnityEngine;

[CreateAssetMenu(fileName = "AntiAnomalyTool Item", menuName = "Inventory/Items/AnomalyToolItem")]
public class AntiAnomalyToolItem : ItemScriptableObject
{
    [Header("Anti-Anomaly Tool Settings")]
    [SerializeField] private float cooldownTime = 2f;
    private float lastUseTime = 0f;
    
    private void Awake()
    {
        itemType = ItemType.Tool;
    }

    public override void Functionality()
    {
        // Проверка кулдауна
        if (Time.time - lastUseTime < cooldownTime)
        {
            Debug.Log("Tool is on cooldown!");
            return;
        }
        
        if (AnomalyToolManager.Instance != null)
        {
            AnomalyToolManager.Instance.FixAnomaly();
            lastUseTime = Time.time;
        }
        else
        {
            Debug.LogWarning("AnomalyToolManager not found in scene!");
        }
    }
    
    // Опционально: Можно добавить метод для проверки готовности инструмента
    public bool IsReady()
    {
        return Time.time - lastUseTime >= cooldownTime;
    }
}