using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct SavedItem
{
    public ItemScriptableObject item;
    public int amount;
    public bool isHotbar;
    public int slotIndex;
}

public class SceneHistory : MonoBehaviour
{
    public static SceneHistory Instance;

    [Header("Настройки")]
    public string LastGameplayScene;
    public GameObject uiPrefab; 

    [Header("Snapshot (Данные чекпоинта)")]
    public List<SavedItem> inventorySnapshot = new List<SavedItem>();
    public int savedSanity = 100;
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

    private IEnumerator WaitAndTakeSnapshot()
    {
        // Ждем немного, чтобы UI и Игрок успели прогрузиться и восстановиться 
        // (если это был переход между уровнями)
        yield return new WaitForSeconds(0.2f);
        
        Debug.Log($"[SceneHistory] Сцена {LastGameplayScene} загружена. Создаем чекпоинт состояния.");
        SaveGameState();
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
    }

    // --- ВОССТАНОВЛЕНИЕ ИЗ ПОСЛЕДНЕГО ЧЕКПОИНТА ---
    public void RestoreGameState()
    {
        if (PersistentObject.Instance == null) return;

        // 1. Sanity
        PlayerStatus playerStatus = Object.FindAnyObjectByType<PlayerStatus>();
        if (playerStatus != null) playerStatus.Sanity = savedSanity;

        // 2. Инвентарь
        InventoryManager inv = PersistentObject.Instance.GetComponentInChildren<InventoryManager>();
        if (inv != null)
        {
            // Сначала очищаем текущий (пустой) инвентарь префаба на всякий случай
            // (хотя префаб и так пустой)
            
            foreach (var s in inventorySnapshot)
            {
                InventorySlot targetSlot = s.isHotbar ? inv.hotbarSlots[s.slotIndex] : inv.slots[s.slotIndex];
                if (targetSlot != null)
                {
                    targetSlot.item = s.item;
                    targetSlot.amount = s.amount;
                    targetSlot.isEmpty = false;
                    targetSlot.SetIcon(s.item.icon);
                }
            }
        }

        // 3. Журнал
        JournalPagesManager journal = PersistentObject.Instance.GetComponentInChildren<JournalPagesManager>();
        if (journal != null)
        {
            journal.collectedPages = new List<Sprite>(savedJournalPages);
            journal.RefreshJournalUI(); 
        }
    }

    public IEnumerator GlobalLoadRoutine(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone) yield return null;

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);

        if (PersistentObject.Instance == null && uiPrefab != null)
        {
            GameObject newUI = Instantiate(uiPrefab);
            newUI.name = "UI_ROOT_REBORN";
            newUI.SetActive(true);

            PersistentObject pObj = newUI.GetComponent<PersistentObject>();
            if (pObj != null)
            {
                PersistentObject.Instance = pObj;
                DontDestroyOnLoad(newUI);
                yield return null; 
                pObj.RefreshInternalLinks();
                
                // Восстанавливаем данные из чекпоинта
                RestoreGameState();
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}