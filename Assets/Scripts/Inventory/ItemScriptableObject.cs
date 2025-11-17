using UnityEngine;

public enum ItemType {Default, Part, Tool, Artefact, Book}
public class ItemScriptableObject : ScriptableObject
{
    public ItemType itemType;
    public GameObject item3D;
    public string itemName;
    public int maximumAmount;
    public string itemDescription;
    public Sprite icon;

}
