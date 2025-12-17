using UnityEngine;
using System.Collections;

public class DarknessTrigger : MonoBehaviour
{
    [Header("Sanity Damage Settings")]
    [SerializeField] private int sanityDamage = 10;
    [SerializeField] private float damageInterval = 2f;

    private bool isPlayerInTrigger = false;
    private Coroutine damageCoroutine;
    private bool wasFlashlightOn = false;

    // Свойство для получения актуального статуса игрока
    private PlayerStatus CurrentPlayerStatus
    {
        get
        {
            if (PersistentObject.Instance != null)
            {
                return PersistentObject.Instance.GetComponentInChildren<PlayerStatus>(true);
            }
            return null;
        }
    }

    // Свойство для получения актуального менеджера фонарика
    private FlashLightManager CurrentFlashlight
    {
        get
        {
            // Используем статический Instance самого менеджера, если он есть
            return FlashLightManager.Instance;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            
            var fl = CurrentFlashlight;
            if (fl != null && fl.IsFlashlightOn())
            {
                wasFlashlightOn = true;
                Debug.Log("Игрок вошел в темноту с фонариком");
            }
            else
            {
                wasFlashlightOn = false;
                Debug.Log("Игрок вошел в темноту без фонарика - урон запущен");
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
            StopDamageCoroutine();
            Debug.Log("Игрок вышел из зоны темноты");
        }
    }

    private void Update()
    {
        if (!isPlayerInTrigger) return;
        
        var fl = CurrentFlashlight;
        if (fl == null) return;

        bool isFlashlightOnNow = fl.IsFlashlightOn();
        
        // Если состояние фонарика изменилось (включили/выключили/сел)
        if (isFlashlightOnNow != wasFlashlightOn)
        {
            if (isFlashlightOnNow)
            {
                StopDamageCoroutine();
                Debug.Log("Фонарик включен - урон остановлен");
            }
            else
            {
                StartDamageCoroutine();
                Debug.Log("Фонарик выключен - урон возобновлен");
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
            
            var fl = CurrentFlashlight;
            var status = CurrentPlayerStatus;

            // Проверяем актуальные ссылки в каждом тике урона
            if (isPlayerInTrigger && fl != null && !fl.IsFlashlightOn() && status != null)
            {
                status.Sanity -= sanityDamage;
                Debug.Log($"Темнота давит! Психика: {status.Sanity}");
            }
        }
        damageCoroutine = null;
    }

    private void OnDrawGizmos()
    {
        if (GetComponent<Collider>() != null)
        {
            Gizmos.color = new Color(0.1f, 0.1f, 0.3f, 0.3f);
            Gizmos.DrawCube(transform.position, GetComponent<Collider>().bounds.size);
        }
    }
}