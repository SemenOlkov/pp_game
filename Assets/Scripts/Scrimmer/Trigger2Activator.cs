using UnityEngine;

public class Trigger2Activator : MonoBehaviour
{
    public Collider trigger1Collider; // Коллайдер триггера 1

    private void Start()
    {
        // Изначально отключить триггер 1
        if (trigger1Collider != null)
        {
            trigger1Collider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Активировать триггер 1 при входе
            if (trigger1Collider != null)
            {
                trigger1Collider.enabled = true;
            }
        }
    }
}