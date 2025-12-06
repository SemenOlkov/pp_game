using UnityEngine;

[CreateAssetMenu(fileName = "AntiAnomalyTool Item", menuName = "Inventory/Items/AnomalyToolItem")]
public class AntiAnomalyToolItem : ItemScriptableObject
{
    [Header("Anti-Anomaly Tool Settings")]
    [SerializeField] private float cooldownTime = 2f;
    
    private void Awake()
    {
        itemType = ItemType.Tool;
    }

    public override void Functionality()
    {
        // Проверка кулдауна
        
        if (AnomalyToolManager.Instance != null)
        {
            AnomalyToolManager.Instance.FixAnomaly();
        }
        else
        {
            Debug.LogWarning("AnomalyToolManager not found in scene!");
        }
    }
    
}