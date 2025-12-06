using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public Animator m_Animator;
    public bool isOpen;

    void Start()
    {
        // Если Animator не назначен, попробуем найти его на том же объекте
        if (m_Animator == null)
        {
            m_Animator = GetComponent<Animator>();
            if (m_Animator == null)
            {
                Debug.LogError("Animator не назначен и не найден на объекте " + gameObject.name);
            }
        }

        if (isOpen && m_Animator != null)
        {
            m_Animator.SetBool("isOpen", true);
        }
    }

    public string GetDescription()
    {
        if (isOpen) return "Press [Left Mouse Button] to <color=red>close</color> the door";
        return "Press [Left Mouse Button] to <color=green>open</color> the door";
    }

    public void Interact()
    {
        isOpen = !isOpen;
        if (m_Animator == null)
        {
            Debug.LogError("Animator не назначен в объекте " + gameObject.name);
            return;
        }

        if (isOpen)
        {
            m_Animator.SetBool("isOpen", true);
        }
        else
        {
            m_Animator.SetBool("isOpen", false);
        }
    }
}
