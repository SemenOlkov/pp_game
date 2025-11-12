using UnityEngine;
using UnityEngine.Rendering;

public class SecondTrigger : MonoBehaviour
{
    public TriggerSystem triggerSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ForceTeleport(other.gameObject));
        }
    }
    
    private System.Collections.IEnumerator ForceTeleport(GameObject player)
    {
        Vector3 originalPosition = player.transform.position;
        
        // Первая попытка
        player.transform.position = triggerSystem.firstTriggerPosition;
        yield return null; // Ждем один кадр
        player.transform.position = triggerSystem.firstTriggerPosition;
        
        Debug.Log($"Финальная позиция: {player.transform.position}");
    }
}