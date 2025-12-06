using UnityEngine;

public class LookedDoor : MonoBehaviour, IInteractable
{
    public Animator m_Animator;
    public bool isOpen;
    public string requiredKeyName; // Название ключа, который нужен

    void Start()
    {
        if (isOpen)
            m_Animator.SetBool("isOpen", true);
    }

    public string GetDescription()
    {
        if (isOpen) return "Press [E] to <color=red>close</color> the door";

        if (!string.IsNullOrEmpty(requiredKeyName) && !PlayerInventory.Instance.HasKey(requiredKeyName))
        {
            return "The door is locked. You need a key.";
        }
        return "Press [E] to <color=green>open</color> the door";
    }

    public void Interact()
    {
        if (!isOpen)
        {
            if (!string.IsNullOrEmpty(requiredKeyName) && !PlayerInventory.Instance.HasKey(requiredKeyName))
            {
                Debug.Log("Дверь заперта. Вам нужен ключ.");
                return;
            }
        }
        isOpen = !isOpen;
        m_Animator.SetBool("isOpen", isOpen);
    }
}