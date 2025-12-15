using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreamerTrigger : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 10f;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private GameObject SingletonUI;
    
    private Camera playerCamera;
    
  private void Start()
{
    // Ищем камеру на игроке
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
        playerCamera = player.GetComponentInChildren<Camera>();
    }
    
    if (playerCamera == null)
    {
        playerCamera = Camera.main;
    }
    
    // Ищем объект с компонентом PersistentObject
    PersistentObject persistentObject = FindObjectOfType<PersistentObject>();
    if (persistentObject != null)
    {
        SingletonUI = persistentObject.gameObject;
    }
    else
    {
        Debug.LogWarning("Не найден объект с компонентом PersistentObject");
    }
}
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SingletonUI.SetActive(false);
            SceneManager.LoadScene("Dead");
            enabled = false;
        }
    }

    private void Update()
    {
        if (playerCamera == null) return;
        
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, raycastDistance, interactionLayer))
        {
            if (hit.collider.gameObject == gameObject)
            {
                SingletonUI.SetActive(false);
                SceneManager.LoadScene("Dead");
                enabled = false; // Отключаем скрипт после срабатывания
            }
        }
    }
}