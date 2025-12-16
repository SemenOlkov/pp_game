using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    public static PersistentObject Instance;

    [Header("Настройки")]
    [SerializeField] private GameObject prefabSource; 
    private CanvasGroup canvasGroup;

    // В PersistentObject.cs и SceneHistory.cs
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Если мы вернулись в меню и нашли старый UI — удаляем новый «дубликат»
            Destroy(gameObject);
        }
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Подписываемся на событие смены сцены
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    
        // Проверяем текущую сцену сразу при запуске
        CheckSceneAndVisibility(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        CheckSceneAndVisibility(scene.name);
    }

    private void CheckSceneAndVisibility(string sceneName)
    {
        // Список сцен, где UI должен быть скрыт
        if (sceneName == "MainMenu" || sceneName == "Dead")
        {
            HideUI();
        }
        else
        {
            ShowUI();
        }
    }

    public void HideUI()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void ShowUI()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ResetToPrefabState()
    {
        if (prefabSource == null) return;

        // 1. Сначала ПЕРЕИМЕНУЕМ и ВЫКЛЮЧИМ старый объект, чтобы он не мешал поиску
        string oldName = gameObject.name;
        gameObject.name = "DELETING_OLD_UI";
        gameObject.SetActive(false); 

        // 2. Создаем НОВЫЙ объект из префаба
        GameObject newObj = Instantiate(prefabSource);
        newObj.name = oldName; // Возвращаем нормальное имя
    
        // 3. Делаем его бессмертным
        DontDestroyOnLoad(newObj);
    
        // 4. ПРИНУДИТЕЛЬНО ВКЛЮЧАЕМ ВСЁ
        newObj.SetActive(true);
        // Проходим по всем вложенным объектам и активируем их
        foreach (Transform child in newObj.transform)
        {
            child.gameObject.SetActive(true);
            // Если у вас есть вложенность глубже, можно сделать рекурсивно, 
            // но для вашей структуры (Root -> Canvases) этого достаточно.
        }

        // 5. Обновляем глобальную статическую ссылку
        Instance = newObj.GetComponent<PersistentObject>();

        // 6. Удаляем старый объект
        Destroy(this.gameObject);
    }

    public void RefreshInternalLinks()
    {
        // 1. Обновляем InventoryManager (камера)
        InventoryManager inv = GetComponentInChildren<InventoryManager>(true);
        if (inv != null) inv.UpdateCameraReference();

        // 2. Обновляем HotbarPanel (игрок)
        HotbarPanel hotbar = GetComponentInChildren<HotbarPanel>(true);
        if (hotbar != null) hotbar.UpdatePlayerReference();

        // 3. Обновляем SanityEffects (игрок)
        SanityEffects sanity = GetComponentInChildren<SanityEffects>(true);
        if (sanity != null) sanity.RefreshPlayerStatus();

        // 4. Обновляем предметы (DragAndDrop)
        DragAndDropItem[] items = GetComponentsInChildren<DragAndDropItem>(true);
        foreach (var item in items) item.UpdatePlayerReference();
        
        Debug.Log("Все канвасы включены, ссылки на игрока и камеру обновлены.");
    }
}