using UnityEngine;

[CreateAssetMenu(fileName = "MoneyItem", menuName = "Inventory/Items/MoneyItem")]
public class MoneyItem : ItemScriptableObject
{
    private void Start()
    {
        itemType = ItemType.Money;
    }

    public override void Functionality()
    {
        
    }
}