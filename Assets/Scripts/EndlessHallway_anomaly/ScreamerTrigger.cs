using UnityEngine;

public class ScreamerTrigger : MonoBehaviour
{
    [Header("Screamer Settings")]
    public GameObject screamerPrefab; // Ссылка на префаб скримера
    public float spawnDistance = 2f; // Дистанция появления перед игроком
    
    private Camera playerCamera;
    private GameObject currentScreamer;
    private bool isLookingAtTrigger = false;

    void Start()
    {
        // Находим главную камеру игрока
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogError("Main camera not found! Make sure there is a camera tagged 'MainCamera' in the scene.");
        }
    }

    void Update()
    {
        CheckIfPlayerLooking();
        
        // Если игрок смотрит на триггер и скример еще не создан
        if (isLookingAtTrigger && currentScreamer == null)
        {
            SpawnScreamer();
        }
        // Если игрок отвел взгляд и скример существует
        // else if (!isLookingAtTrigger && currentScreamer != null)
        // {
        //     DestroyScreamer();
        // }
    }

    void CheckIfPlayerLooking()
    {
        if (playerCamera == null) return;
        
        RaycastHit hit;
        Vector3 rayDirection = playerCamera.transform.forward;
        Vector3 rayOrigin = playerCamera.transform.position;

        // Пускаем луч из камеры вперед
        if (Physics.Raycast(rayOrigin, rayDirection, out hit))
        {
            // Проверяем, попал ли луч в этот триггер
            if (hit.collider.gameObject == this.gameObject)
            {
                isLookingAtTrigger = true;
            }
            else
            {
                isLookingAtTrigger = false;
            }
        }
        else
        {
            isLookingAtTrigger = false;
        }
    }

    void SpawnScreamer()
    {
        if (screamerPrefab == null)
        {
            Debug.LogWarning("Screamer prefab is not assigned!");
            return;
        }

        // Создаем префаб перед камерой игрока с фиксированной высотой -3
        Vector3 spawnPosition = playerCamera.transform.position + 
                               playerCamera.transform.forward * spawnDistance;
        spawnPosition.y = -2f; // Фиксированная высота
        
        currentScreamer = Instantiate(screamerPrefab, spawnPosition, Quaternion.identity);
        
        // Увеличиваем размер в 2 раза
        currentScreamer.transform.localScale = Vector3.one * 2.5f;
        
        // Поворачиваем объект к игроку
        // currentScreamer.transform.LookAt(playerCamera.transform);
        
        Debug.Log("Screamer appeared!");
    }

    void DestroyScreamer()
    {
        if (currentScreamer != null)
        {
            Destroy(currentScreamer);
            currentScreamer = null;
            Debug.Log("Screamer destroyed!");
        }
    }

    // Визуализация луча в редакторе для отладки
    void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.red;
            Vector3 rayOrigin = playerCamera.transform.position;
            Vector3 rayEnd = rayOrigin + playerCamera.transform.forward * 10f;
            Gizmos.DrawLine(rayOrigin, rayEnd);
        }
    }
}