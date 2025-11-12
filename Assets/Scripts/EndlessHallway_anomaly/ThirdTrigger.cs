using UnityEngine;

public class ThirdTrigger : MonoBehaviour
{
    public TriggerSystem triggerSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Деактивируем второй триггер и себя
            triggerSystem.secondTrigger.SetActive(false);
            gameObject.SetActive(false);
            
            Debug.Log("Третий триггер активирован! Второй триггер и этот триггер деактивированы.");
        }
    }
}