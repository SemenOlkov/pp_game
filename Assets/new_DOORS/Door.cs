using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public bool isOpen = false;
    public float openAngle = 90f; // угол открыти€ двери
    public float closeAngle = 0f; // угол закрыти€ двери
    public float smoothSpeed = 5f;

    private Quaternion targetRotation;

    void Start()
    {
        // ”становим начальную позицию
        targetRotation = Quaternion.Euler(0, closeAngle, 0);
        // ≈сли дверь уже открыта
        if (isOpen)
        {
            targetRotation = Quaternion.Euler(0, openAngle, 0);
            transform.localRotation = targetRotation;
        }
    }

    void Update()
    {
        // ѕлавное вращение к целевому углу
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smoothSpeed);
    }

    public string GetDescription()
    {
        if (isOpen)
            return "Press [Left Mouse Button] to <color=red>close</color> the door";
        else
            return "Press [Left Mouse Button] to <color=green>open</color> the door";
    }

    public void Interact()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            targetRotation = Quaternion.Euler(0, openAngle, 0);
        }
        else
        {
            targetRotation = Quaternion.Euler(0, closeAngle, 0);
        }
    }
}