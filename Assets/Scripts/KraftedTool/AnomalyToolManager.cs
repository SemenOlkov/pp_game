using UnityEngine;

public class AnomalyToolManager : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float raycastDistance = 10f;
    
    public static AnomalyToolManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Если камера не назначена, пытаемся найти главную камеру
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    public void FixAnomaly()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("Camera not assigned to AnomalyToolManager!");
            return;
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
    
}