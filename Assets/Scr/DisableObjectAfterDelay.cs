using UnityEngine;

public class DisableObjectAfterDelay : MonoBehaviour
{
    void Start()
    {
        Invoke("DisableObject", 1f); // вызываем функцию через 1 секунду
    }

    void DisableObject()
    {
        gameObject.SetActive(false);
    }
}