using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    private HashSet<string> keys = new HashSet<string>();
    private HashSet<string> items = new HashSet<string>(); // добавим для предметов, в т.ч. фонарик

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddKey(string keyName)
    {
        keys.Add(keyName);
        Debug.Log($"Key {keyName} добавлен в инвентарь");
    }

    public bool HasKey(string keyName)
    {
        return keys.Contains(keyName);
    }

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        Debug.Log($"Предмет {itemName} добавлен в инвентарь");
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }
}