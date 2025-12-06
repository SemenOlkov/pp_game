using UnityEngine;
using System.Collections;

public class DarknessTrigger : MonoBehaviour
{
    [Header("References")]
    public PlayerStatus playerStatus;
    
    [Header("Sanity Damage Settings")]
    [SerializeField] private int sanityDamage = 10;
    [SerializeField] private float damageInterval = 2f; // Интервал между уроном в секундах
    
    private bool isPlayerInTrigger = false;
    private Coroutine damageCoroutine;
    private FlashLightManager flashlightManager;
    private bool wasFlashlightOn = false;

    private void Start()
    {
        // Получаем ссылку на FlashLightManager
        flashlightManager = FlashLightManager.Instance;
        
        if (flashlightManager == null)
        {
            Debug.LogWarning("FlashLightManager not found in scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Проверяем PlayerStatus
            if (playerStatus == null)
            {
                playerStatus = other.GetComponent<PlayerStatus>();
                if (playerStatus == null)
                {
                    Debug.LogWarning("PlayerStatus not found on player!");
                    return;
                }
            }

            isPlayerInTrigger = true;
            
            // Проверяем состояние фонарика при входе
            if (flashlightManager != null && flashlightManager.IsFlashlightOn())
            {
                wasFlashlightOn = true;
                Debug.Log("Игрок вошел в зону темноты с фонариком");
            }
            else
            {
                wasFlashlightOn = false;
                Debug.Log("Игрок вошел в зону темноты без фонарика - начинает терять психику!");
                // Запускаем корутину нанесения урона
                StartDamageCoroutine();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            wasFlashlightOn = false;
            
            // Останавливаем корутину при выходе из триггера
            StopDamageCoroutine();
            
            Debug.Log("Игрок вышел из зоны темноты");
        }
    }

    private void Update()
    {
        if (!isPlayerInTrigger || flashlightManager == null) return;
        
        bool isFlashlightOnNow = flashlightManager.IsFlashlightOn();
        
        // Если состояние фонарика изменилось
        if (isFlashlightOnNow != wasFlashlightOn)
        {
            // Если фонарик включили - останавливаем урон
            if (isFlashlightOnNow && damageCoroutine != null)
            {
                StopDamageCoroutine();
                Debug.Log("Фонарик включен - урон остановлен");
            }
            // Если фонарик выключили (или разрядился) - запускаем урон
            else if (!isFlashlightOnNow && damageCoroutine == null && isPlayerInTrigger)
            {
                Debug.Log("Фонарик выключен - урон возобновлен");
                StartDamageCoroutine();
            }
            
            wasFlashlightOn = isFlashlightOnNow;
        }
    }

    private void StartDamageCoroutine()
    {
        if (damageCoroutine == null && isPlayerInTrigger)
        {
            damageCoroutine = StartCoroutine(ApplyContinuousDamage());
        }
    }

    private void StopDamageCoroutine()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private IEnumerator ApplyContinuousDamage()
    {
        while (isPlayerInTrigger)
        {
            yield return new WaitForSeconds(damageInterval);
            
            // Проверяем, что игрок все еще в триггере и фонарик выключен
            if (isPlayerInTrigger && 
                flashlightManager != null && 
                !flashlightManager.IsFlashlightOn() && 
                playerStatus != null)
            {
                playerStatus.Sanity -= sanityDamage;
                Debug.Log($"Психика уменьшена на {sanityDamage}. Текущая психика: {playerStatus.Sanity}");
            }
        }
        
        // Корутина завершилась - обнуляем ссылку
        damageCoroutine = null;
    }

    // Для дебага - отображение триггера в редакторе
    private void OnDrawGizmos()
    {
        if (GetComponent<Collider>() != null)
        {
            Gizmos.color = new Color(0.1f, 0.1f, 0.3f, 0.3f);
            Gizmos.DrawCube(transform.position, GetComponent<Collider>().bounds.size);
        }
    }
}