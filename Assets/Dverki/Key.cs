using UnityEngine;

public class Key : MonoBehaviour, IInteractable
{
    public string keyName = "GoldenKey"; // Название ключа

    public void Interact()
    {
        PlayerInventory.Instance.AddKey(keyName);
        Destroy(gameObject);
    }

    public string GetDescription()
    {
        return "Press [E] to pick up the key.";
    }
}