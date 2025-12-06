using UnityEngine;

public class DisableCameraAfterDelay : MonoBehaviour
{
    public Camera targetCamera; // Камера, которую нужно отключить

    void Start()
    {
        Invoke("DisableCamera", 1f); // вызов функции через 1 секунду
    }

    void DisableCamera()
    {
        if (targetCamera != null)
        {
            targetCamera.enabled = false;
        }
    }
}