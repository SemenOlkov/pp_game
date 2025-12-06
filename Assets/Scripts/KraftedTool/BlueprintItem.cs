using UnityEngine;

[CreateAssetMenu(fileName = "Blueprint Item", menuName = "Inventory/Items/NewBlueprintItem")]
public class BlueprintItem : ItemScriptableObject
{
    [Header("Blueprint Pages")]
    public Sprite[] blueprintPages;

    private void Start()
    {
        itemType = ItemType.Blueprint;
    }

    public override void Functionality()
    {
        base.Functionality();
        
        Debug.Log($"Reading blueprint: {itemName}");
        Debug.Log($"Pages: {blueprintPages.Length}");
        
        OpenBlueprintUI();
    }

    private void OpenBlueprintUI()
    {
        BlueprintInputManager inputManager = FindObjectOfType<BlueprintInputManager>();
        if (inputManager != null)
        {
            inputManager.OpenBlueprintUI(this);
        }
        else
        {
            Debug.LogError("BlueprintInputManager not found in scene!");
        }
    }
}