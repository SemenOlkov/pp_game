using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreamerTrigger : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 10f;
    [SerializeField] private LayerMask interactionLayer;
    
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
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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
                SceneManager.LoadScene("Dead");
                enabled = false; // Отключаем скрипт после срабатывания
            }
        }
    }
}