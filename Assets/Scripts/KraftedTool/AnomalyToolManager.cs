using UnityEngine;

public class AnomalyToolManager : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float raycastDistance = 10f;
    
    public static AnomalyToolManager Instance { get; private set; }

    private void Start()
    {
        // Назначаем Instance только один раз
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return; // Прерываем выполнение, если объект будет уничтожен
        }
        
        // Ищем камеру в Start вместо Awake
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        // Дополнительная проверка на случай, если камера все еще не найдена
        if (playerCamera == null)
        {
            Debug.LogError("Camera not found! Make sure there is an active camera with MainCamera tag in the scene.");
        }
    }

    public void FixAnomaly()
    {
        // Проверяем наличие камеры перед использованием
        if (playerCamera == null)
        {
            // Попытка найти камеру в реальном времени (на случай, если она появилась позже)
            playerCamera = Camera.main;
            
            if (playerCamera == null)
            {
                Debug.LogWarning("Camera not assigned to AnomalyToolManager and no MainCamera found!");
                return;
            }
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("AnomalyObject"))
            {
                Destroy(hit.collider.gameObject);
                Debug.Log("Anomaly fixed!");
                
                // Дополнительные эффекты можно добавить здесь
                // Например, звук, частицы и т.д.
            }
        }
    }
    
    // Необязательно, но полезно: очистка статического Instance при уничтожении
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}