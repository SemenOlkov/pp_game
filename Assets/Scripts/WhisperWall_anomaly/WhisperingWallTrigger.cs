using UnityEngine;
using System.Collections;

public class WhisperingWallTrigger : MonoBehaviour
{
    [Header("Sanity Damage")]
    [SerializeField] private int sanityDamage = 15; 
    [SerializeField] private float damageInterval = 1f; 

    private bool isPlayerInTrigger = false;
    private Coroutine damageCoroutine;

    // Свойство для получения актуальной ссылки на PlayerStatus
    // Оно всегда найдет новый компонент, даже если UI был пересоздан
    private PlayerStatus CurrentPlayerStatus
    {
        get
        {
            if (PersistentObject.Instance != null)
            {
                // Ищем компонент внутри текущего живого синглтона UI
                return PersistentObject.Instance.GetComponentInChildren<PlayerStatus>(true);
            }
            return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            
            // Запускаем корутину. Теперь нам не нужно проверять ссылку тут,
            // корутина сама будет брать "живой" объект в цикле.
            if (damageCoroutine == null)
            {
                damageCoroutine = StartCoroutine(ApplyContinuousDamage());
            }
            
            Debug.Log("Игрок вошел в зону шепчущей стены.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
            
            Debug.Log("Игрок вышел из зоны шепчущей стены.");
        }
    }

    private IEnumerator ApplyContinuousDamage()
    {
        while (isPlayerInTrigger)
        {
            yield return new WaitForSeconds(damageInterval);
            
            // Получаем актуальную ссылку через свойство
            PlayerStatus status = CurrentPlayerStatus;

            if (status != null)
            {
                status.Sanity -= sanityDamage;
                Debug.Log($"Шепчущая стена наносит урон. Текущая психика: {status.Sanity}");
            }
            else
            {
                Debug.LogWarning("[WhisperingWall] PlayerStatus не найден в PersistentObject!");
            }
        }
        damageCoroutine = null;
    }
}