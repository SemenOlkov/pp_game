using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq; 

[System.Serializable]
public struct SavedItem
{
    public ItemScriptableObject item;
    public int amount;
    public bool isHotbar;
    public int slotIndex;
}

[System.Serializable]
public class SaveData
{
    public string sceneName;
    public string saveDate;
    public int sanity;
    public float battery;
    public List<SavedItem> inventory;
    public List<string> journalPageNames; // Имена спрайтов страниц
}

public class SceneHistory : MonoBehaviour
{
    public static SceneHistory Instance;
    public bool isLoadingFromSave = false;

    [Header("Настройки")]
    public string LastGameplayScene;
    public GameObject uiPrefab; 

    [Header("Snapshot (Данные чекпоинта)")]
    public List<SavedItem> inventorySnapshot = new List<SavedItem>();
    public int savedSanity = 100;
    public float savedFlashlightBattery = 100f;
    public List<Sprite> savedJournalPages = new List<Sprite>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Если загруженная сцена — игровая (не меню и не экран смерти)
        if (scene.name != "Dead" && scene.name != "MainMenu")
        {
            LastGameplayScene = scene.name;
            
            // Запускаем сохранение данных "на старте"
            // Используем корутину, чтобы подождать, пока объекты инициализируются
            StartCoroutine(WaitAndTakeSnapshot());
        }
    }

    public void SaveToFile(string fileName)
    {
        SaveData data = new SaveData();
        data.sceneName = LastGameplayScene;
        data.saveDate = System.DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        data.sanity = savedSanity;
        data.battery = savedFlashlightBattery;
        data.inventory = new List<SavedItem>(inventorySnapshot);
        
        // Спрайты нельзя сохранить напрямую, сохраняем их имена 
        data.journalPageNames = savedJournalPages.Select(s => s.name).ToList();

        string json = JsonUtility.ToJson(data, true);
        // Сохраняем в системную папку игры
        string path = Path.Combine(Application.persistentDataPath, fileName + ".json");
        File.WriteAllText(path, json);
        
        Debug.Log($"[SceneHistory] Файл сохранения создан: {path}");
    }

    public void LoadFromFile(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName + ".json");

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SceneHistory] Файл сохранения {fileName} не найден по пути: {path}");
            return;
        }

        try
        {
            // 1. Читаем текст из файла
            string json = File.ReadAllText(path);

            // 2. Превращаем JSON обратно в объект данных
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // 3. Заполняем переменные снапшота данными из файла
            inventorySnapshot = data.inventory;
            savedSanity = data.sanity;
            savedFlashlightBattery = data.battery;
        
            savedJournalPages.Clear();

        foreach (string pageName in data.journalPageNames)
        {
            Sprite loadedSprite = null;

            // Список всех подпапок внутри Resources, где могут лежать страницы
            string[] folders = { "AnomalyJournalPages/", "Notes/" };

            foreach (string folder in folders)
            {
                // Пытаемся загрузить
                loadedSprite = Resources.Load<Sprite>(folder + pageName);
        
                // Если нашли — выходим из внутреннего цикла
                if (loadedSprite != null) 
                {
                    Debug.Log($"[SceneHistory] Страница {pageName} успешно найдена в папке: {folder}");
                    break; 
                }
            }

            if (loadedSprite != null)
            {
                savedJournalPages.Add(loadedSprite);
            }
            else
            {
                Debug.LogError($"[SceneHistory] КРИТИЧЕСКАЯ ОШИБКА: Спрайт '{pageName}' не найден ни в одной из папок Resources!");
            }
        }   

            isLoadingFromSave = true; 

            Debug.Log($"[SceneHistory] Флаг isLoadingFromSave установлен. Начинаем загрузку сцены.");

            // 4. Запускаем загрузку сцены, которая была указана в сохранении
            StartCoroutine(GlobalLoadRoutine(data.sceneName));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SceneHistory] Ошибка при чтении файла сохранения: {e.Message}");
        }
    }

    private IEnumerator WaitAndTakeSnapshot()
{
        // Ждем, пока RestoreGameState точно закончит работу
        yield return new WaitForSeconds(0.4f); 

        if (isLoadingFromSave)
        {
            Debug.Log("[SceneHistory] Загрузка обнаружена. Отмена перезаписи автосейва.");
            isLoadingFromSave = false; // Выключаем защиту до следующего перехода в дверь
            yield break; // ВЫХОДИМ, не вызывая SaveGameState()
        }

        // Если мы здесь — значит это обычный проход через дверь, можно сохранять
        SaveGameState();
        SaveToFile("Autosave");
    }

    // --- МЕТОД СНЯТИЯ СНИМКА (ЧЕКПОИНТ) ---
    public void SaveGameState()
    {
        if (PersistentObject.Instance == null) return;

        inventorySnapshot.Clear();
        InventoryManager inv = PersistentObject.Instance.GetComponentInChildren<InventoryManager>();
        if (inv != null)
        {
            for (int i = 0; i < inv.slots.Count; i++)
                if (!inv.slots[i].isEmpty) inventorySnapshot.Add(new SavedItem { item = inv.slots[i].item, amount = inv.slots[i].amount, isHotbar = false, slotIndex = i });
            
            for (int i = 0; i < inv.hotbarSlots.Count; i++)
                if (!inv.hotbarSlots[i].isEmpty) inventorySnapshot.Add(new SavedItem { item = inv.hotbarSlots[i].item, amount = inv.hotbarSlots[i].amount, isHotbar = true, slotIndex = i });
        }

        PlayerStatus playerStatus = Object.FindAnyObjectByType<PlayerStatus>();
        if (playerStatus != null) savedSanity = playerStatus.Sanity;

        JournalPagesManager journal = PersistentObject.Instance.GetComponentInChildren<JournalPagesManager>();
        if (journal != null) savedJournalPages = new List<Sprite>(journal.collectedPages);
        
        Debug.Log("[SceneHistory] Состояние инвентаря и Sanity зафиксировано для текущей сцены.");

            if (FlashLightManager.Instance != null)
            {
                savedFlashlightBattery = FlashLightManager.Instance.currentBattery;
            }
        }

        // --- ВОССТАНОВЛЕНИЕ ИЗ ПОСЛЕДНЕГО ЧЕКПОИНТА ---
        public void RestoreGameState()
        {
        if (PersistentObject.Instance == null) return;

        // 1. Sanity
        PlayerStatus playerStatus = Object.FindAnyObjectByType<PlayerStatus>();
        if (playerStatus != null) playerStatus.Sanity = savedSanity;

        // 2. Инвентарь
        InventoryManager inv = PersistentObject.Instance.GetComponentInChildren<InventoryManager>(true);
        if (inv != null)
        {
            // ПРИНУДИТЕЛЬНО находим слоты, если скрипт инвентаря еще не проснулся
            if (inv.slots == null || inv.slots.Count == 0)
             inv.slots = inv.GetComponentsInChildren<InventorySlot>(true).Where(s => !s.isHotbarSlot).ToList();
        
            if (inv.hotbarSlots == null || inv.hotbarSlots.Count == 0)
                inv.hotbarSlots = inv.GetComponentsInChildren<InventorySlot>(true).Where(s => s.isHotbarSlot).ToList();

            // Очищаем
            foreach (var s in inv.slots) s.ClearSlot();
            foreach (var s in inv.hotbarSlots) s.ClearSlot();

            // Заполняем
            foreach (var s in inventorySnapshot)
            {
                var targetList = s.isHotbar ? inv.hotbarSlots : inv.slots;
                if (s.slotIndex >= 0 && s.slotIndex < targetList.Count)
                {
                    InventorySlot slot = targetList[s.slotIndex];
                    slot.item = s.item;
                    slot.amount = s.amount;
                    slot.isEmpty = false;
                    slot.SetIcon(s.item.icon);
                }
            }
        }

        // 3. ЖУРНАЛ (Исправлено)
        JournalPagesManager journal = PersistentObject.Instance.GetComponentInChildren<JournalPagesManager>(true);
        if (journal != null)
        {
            // Прямое присваивание списка
            journal.collectedPages = new List<Sprite>(savedJournalPages);
        
            // Если в скрипте журнала есть список слотов для страниц, его тоже нужно проверить
            // Но обычно RefreshJournalUI сам перерисовывает всё на основе collectedPages
            journal.RefreshJournalUI(); 
            Debug.Log($"[SceneHistory] Журнал восстановлен: {savedJournalPages.Count} страниц.");
        }

        // 4. Фонарик
        if (FlashLightManager.Instance != null)
        {
            FlashLightManager.Instance.currentBattery = savedFlashlightBattery;
            FlashLightManager.Instance.CheckInventoryForFlashlight();
        }
    }

    public IEnumerator GlobalLoadRoutine(string sceneName)
{
    isLoadingFromSave = true;
    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
    while (!asyncLoad.isDone) yield return null;

    // Ждем, пока Unity проинициализирует все новые объекты на сцене
    yield return new WaitForEndOfFrame();
    yield return new WaitForSeconds(0.2f); // КРИТИЧЕСКАЯ ПАУЗА

    if (PersistentObject.Instance == null)
    {
        PersistentObject found = Object.FindAnyObjectByType<PersistentObject>();
        if (found != null) PersistentObject.Instance = found;
        else if (uiPrefab != null)
        {
            GameObject newUI = Instantiate(uiPrefab);
            newUI.name = "UI_ROOT_REBORN";
            PersistentObject.Instance = newUI.GetComponent<PersistentObject>();
            DontDestroyOnLoad(newUI);
        }
    }

    if (PersistentObject.Instance != null)
    {
        PersistentObject.Instance.RefreshInternalLinks();
        // Теперь все скрипты точно готовы принимать данные
        RestoreGameState();
    }
}

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}