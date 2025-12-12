using UnityEngine;

[CreateAssetMenu(fileName = "KeyItem", menuName = "Inventory/Items/KeyItem")]
public class KeyItem : ItemScriptableObject
{   
    public string keyCode;

    private void Start()
    {
        itemType = ItemType.Key;
    }

    public override void Functionality()
    {
        base.Functionality();
        Debug.Log("Использован ключ.");
        Debug.Log(keyCode);
    }

}