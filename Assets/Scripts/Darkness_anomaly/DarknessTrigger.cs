using UnityEngine;
using System.Collections;

public class DarknessTrigger : MonoBehaviour
{
    [Header("References")]
    public PlayerBinds playerBinds;
    public PlayerStatus playerStatus;

    [Header("Sanity Damage Settings")]
    [SerializeField] private int sanityDamage = 10;
    [SerializeField] private float damageInterval = 2f; // Интервал между уроном в секундах

    private bool isPlayerInTrigger = false;
    private Coroutine damageCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Проверяем PlayerBinds
            if (playerBinds == null)
            {
                playerBinds = other.GetComponent<PlayerBinds>();
                if (playerBinds == null)
                {
                    Debug.LogWarning("PlayerBinds not found on player!");
                    return;
                }
            }

            // Проверяем PlayerStatus
            if (playerStatus == null)
            {
                Debug.LogWarning("PlayerStatus reference is not set!");
                return;
            }

            isPlayerInTrigger = true;
            
            // Проверяем состояние фонарика при входе
            if (playerBinds.IsFlashlightOn())
            {
                Debug.Log("Игрок вошел в зону темноты с фонариком");
            }
            else
            {
                Debug.Log("Игрок вошел в зону темноты без фонарика - начинает терять психику!");
                // Запускаем корутину нанесения урона
                damageCoroutine = StartCoroutine(ApplyContinuousDamage());
            }
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
            
            Debug.Log("Игрок вышел из зоны темноты");
        }
    }

    private IEnumerator ApplyContinuousDamage()
    {
        while (isPlayerInTrigger)
        {
            yield return new WaitForSeconds(damageInterval);
            
            // Проверяем, что игрок все еще в триггере и фонарик выключен
            if (isPlayerInTrigger && playerBinds != null && !playerBinds.IsFlashlightOn())
            {
                playerStatus.Sanity -= sanityDamage;
                Debug.Log($"Психика уменьшена на {sanityDamage}. Текущая психика: {playerStatus.Sanity}");
            }
        }
    }

    // Опционально: обработка включения/выключения фонарика во время нахождения в триггере
    private void Update()
    {
        if (isPlayerInTrigger && playerBinds != null)
        {
            // Если фонарик включили - останавливаем урон
            if (playerBinds.IsFlashlightOn() && damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
                Debug.Log("Фонарик включен - урон остановлен");
            }
            // Если фонарик выключили - запускаем урон
            else if (!playerBinds.IsFlashlightOn() && damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(ApplyContinuousDamage());
                Debug.Log("Фонарик выключен - урон возобновлен");
            }
        }
    }
}