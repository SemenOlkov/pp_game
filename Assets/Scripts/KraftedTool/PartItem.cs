using UnityEngine;

[CreateAssetMenu(fileName = "Part Item", menuName = "Inventory/Items/PartItem")]
public class PartItem : ItemScriptableObject
{
    
    private void Start()
    {
        itemType = ItemType.Part;
    }

    public override void Functionality()
    {
    
    }
}