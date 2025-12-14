using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorDefault : MonoBehaviour
{
    [Header("Door Settings")]
    public bool canGo = false;
    public string sceneName = "Dead";

    [Header("UI Settings")]
    [SerializeField] private GameObject goText; // Текст подсказки "Можно перейти"
    [SerializeField] private GameObject cantGoText; // Текст подсказки "Нельзя перейти"
    
    [Header("Raycast Settings")]
    [SerializeField] private float interactionDistance = 7f; // Дистанция взаимодействия
    [SerializeField] private LayerMask interactionLayer; // Слой для взаимодействия

    private Camera playerCamera;
    private bool isHovered = false;

    void Start()
    {
        // Находим камеру
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerCamera = player.GetComponentInChildren<Camera>();
            }
        }
        
        HideAllHints();
        
        // Отладка
        Debug.Log($"{gameObject.name}: Скрипт и коллайдер на одном объекте");
    }

    void Update()
    {
        HandleRaycast();
        HandleInput();
    }

    private void HandleRaycast()
    {
        if (playerCamera == null) return;

        // Луч из центра экрана
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Визуализация луча
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.yellow, 0.1f);

        // Проверяем пересечение луча
        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            // ПРЯМАЯ ПРОВЕРКА: луч попал в этот же GameObject
            if (hit.collider.gameObject == gameObject)
            {
                if (!isHovered)
                {
                    isHovered = true;
                    ShowHint();
                    Debug.Log($"Навели на дверь: {gameObject.name}");
                }
                return;
            }
        }
        
        // Если луч перестал попадать
        if (isHovered)
        {
            isHovered = false;
            HideAllHints();
            Debug.Log($"Ушли от двери: {gameObject.name}");
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) && isHovered)
        {
            TryLoadScene();
        }
    }

    private void ShowHint()
    {
        HideAllHints();
        
        if (canGo)
        {
            if (goText != null)
            {
                goText.SetActive(true);
                Debug.Log("Показали: МОЖНО ИДТИ");
            }
        }
        else
        {
            if (cantGoText != null)
            {
                cantGoText.SetActive(true);
                Debug.Log("Показали: НЕЛЬЗЯ ИДТИ");
            }
        }
    }

    private void HideAllHints()
    {
        if (goText != null)
            goText.SetActive(false);
        if (cantGoText != null)
            cantGoText.SetActive(false);
    }

    private void TryLoadScene()
    {
        if (canGo && !string.IsNullOrEmpty(sceneName))
        {
            Debug.Log($"Загружаем сцену: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
        else if (!canGo)
        {
            Debug.Log("Дверь закрыта! canGo = false");
        }
        else
        {
            Debug.LogError($"sceneName не установлен на двери {gameObject.name}");
        }
    }

    // Публичные методы
    public void UnlockDoor()
    {
        canGo = true;
        if (isHovered) ShowHint();
    }

    public void LockDoor()
    {
        canGo = false;
        if (isHovered) ShowHint();
    }

    // Вспомогательные методы
    public bool IsHovered() => isHovered;
    public bool CanGo() => canGo;
    public void SetScene(string name) => sceneName = name;
}