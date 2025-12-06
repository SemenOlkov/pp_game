using UnityEngine;

public class RightDoor : MonoBehaviour
{
    public Animator m_Animator;

    // Метод вызывается из левой створки для синхронизации
    public void SetDoorState(bool open)
    {
        m_Animator.SetBool("isOpen", open);
    }
}