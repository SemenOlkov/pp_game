using UnityEngine;

public class FirstTrigger : MonoBehaviour
{
    public TriggerSystem triggerSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Активируем второй и третий триггеры
            triggerSystem.secondTrigger.SetActive(true);
            triggerSystem.thirdTrigger.SetActive(true);
            
            // Деактивируем себя
            gameObject.SetActive(false);
            
            Debug.Log("Первый триггер активирован! Второй и третий триггеры теперь активны.");
        }
    }
}