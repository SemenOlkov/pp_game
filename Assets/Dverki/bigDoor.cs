using UnityEngine;

public class bigDoor : MonoBehaviour, IInteractable
{
    public Animator leftAnimator;
    public Animator rightAnimator;
    public bool isOpen;

    void Start()
    {
        // Инициализация состояния обеих створок
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
        return "Press [E] to <color=green>open</color> the door";
    }

    public void Interact()
    {
        isOpen = !isOpen;
        // синхронное управление обеими створками
        leftAnimator.SetBool("isOpen", isOpen);
        rightAnimator.SetBool("isOpen", isOpen);
    }
}