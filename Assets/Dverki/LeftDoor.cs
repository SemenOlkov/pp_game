using UnityEngine;

public class LeftDoor : MonoBehaviour, IInteractable
{
    public Animator m_Animator;
    public RightDoor rightDoor; // ссылка на правую створку

    public bool isOpen;

    void Start()
    {
        if (isOpen)
        {
            m_Animator.SetBool("isOpen", true);
        }
    }

    public string GetDescription()
    {
        if (isOpen) return "Press [E] to <color=red>close</color> the door";
        return "Press [E] to <color=green>open</color> the door";
    }

    public void Interact()
    {
        isOpen = !isOpen;
        m_Animator.SetBool("isOpen", isOpen);

        // синхронизируем правую створку
        if (rightDoor != null)
        {
            rightDoor.SetDoorState(isOpen);
        }
    }
}