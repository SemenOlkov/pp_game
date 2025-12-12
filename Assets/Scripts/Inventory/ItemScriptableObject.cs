using UnityEngine;

public enum ItemType {Default, Part, Tool, Book, Flashlight, Battery, Blueprint, Page, Pills, Money, Crowbar, Key}
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
