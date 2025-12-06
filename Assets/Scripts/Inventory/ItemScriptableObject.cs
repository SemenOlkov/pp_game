using UnityEngine;

public enum ItemType {Default, Part, Tool, Artefact, Book, Flashlight, Battery, Blueprint}
public class ItemScriptableObject : ScriptableObject
{
    public ItemType itemType;
    public GameObject item3D;
    public string itemName;
    public int maximumAmount;
    public string itemDescription;
    public Sprite icon;
    public bool isConsumable = false;

    public virtual void Functionality()
    {
        // Базовая реализация - может быть пустой
        Debug.Log($"Using {itemName}");
    }
}
