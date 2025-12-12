using UnityEngine;

[CreateAssetMenu(fileName = "CrowbarItem", menuName = "Inventory/Items/CrowbarItem")]
public class CrowbarItem : ItemScriptableObject
{

    private void Start()
    {
        itemType = ItemType.Crowbar;
    }

    public override void Functionality()
    {
        base.Functionality();
        Debug.Log("Использована монтировка");
    }

}