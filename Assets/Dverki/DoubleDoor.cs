using UnityEngine;

public class DoubleDoor : MonoBehaviour, IInteractable
{
    public Animator leftAnimator;   // Аниматор для левой створки
    public Animator rightAnimator;  // Аниматор для правой створки
    public bool isOpen;
    public string requiredKeyName;  // Название ключа, если дверь заперта

    void Start()
    {
        // Инициализация состояния створок
        if (isOpen)
        {
            leftAnimator.SetBool("isOpen", true);
            rightAnimator.SetBool("isOpen", true);
        }
        else
        {
            leftAnimator.SetBool("isOpen", false);
            rightAnimator.SetBool("isOpen", false);
        }
    }

    public string GetDescription()
    {
        if (isOpen)
            return "Press [E] to <color=red>close</color> the door";

        if (!string.IsNullOrEmpty(requiredKeyName) && !PlayerInventory.Instance.HasKey(requiredKeyName))
        {
            return "The door is locked. You need a key.";
        }
        return "Press [E] to <color=green>open</color> the door";
    }

    public void Interact()
    {
        // Проверка на ключ
        if (!isOpen)
        {
            if (!string.IsNullOrEmpty(requiredKeyName) && !PlayerInventory.Instance.HasKey(requiredKeyName))
            {
                Debug.Log("Дверь заперта. Вам нужен ключ.");
                return;
            }
        }

        // Переключение состояния
        isOpen = !isOpen;
        // Анимация обеих створок
        leftAnimator.SetBool("isOpen", isOpen);
        rightAnimator.SetBool("isOpen", isOpen);
    }
}