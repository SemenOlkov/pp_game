using UnityEngine;
using System.Collections;

public class WhisperingWallTrigger : MonoBehaviour
{
    [Header("Sanity Damage")]
    [SerializeField] private int sanityDamage = 15; // Урон по психике от шепчущей стены
    [SerializeField] private float damageInterval = 1f; // Интервал между уроном в секундах

    [Header("Player Status Reference")]
    [SerializeField] private PlayerStatus playerStatus; // Установите в инспекторе

    private bool isPlayerInTrigger = false;
    private Coroutine damageCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Проверяем, установлена ли ссылка на PlayerStatus
            if (playerStatus == null)
            {
                Debug.LogWarning("PlayerStatus reference is not set! Please assign it in the inspector.");
                return;
            }

            isPlayerInTrigger = true;
            
            // Запускаем корутину нанесения урона
            damageCoroutine = StartCoroutine(ApplyContinuousDamage());
            
            Debug.Log("Игрок вошел в зону шепчущей стены - начинает терять психику!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            
            // Останавливаем корутину при выходе из триггера
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
            
            Debug.Log("Игрок вышел из зоны шепчущей стены");
        }
    }

    private IEnumerator ApplyContinuousDamage()
    {
        while (isPlayerInTrigger)
        {
            yield return new WaitForSeconds(damageInterval);
            
            // Наносим урон по психике каждую секунду
            if (isPlayerInTrigger && playerStatus != null)
            {
                playerStatus.Sanity -= sanityDamage;
                Debug.Log($"Шепчущая стена! Психика уменьшена на {sanityDamage}. Текущая психика: {playerStatus.Sanity}");
            }
        }
    }
}