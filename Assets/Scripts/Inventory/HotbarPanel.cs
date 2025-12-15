using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class HotbarPanel : MonoBehaviour
{
    [Header("Hotbar Settings")]
    public List<InventorySlot> slots = new List<InventorySlot>();
    public int selectedSlotIndex = 0;
    public Color selectedColor = new Color(1f, 1f, 1f, 1f);
    public Color normalColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    public Transform player;

    [Header("Usage Settings")]
    public float useCooldown = 0.5f; // Задержка между использованиями
    private float lastUseTime = 0f;

    private void Awake()
    {
        // Подписываемся на событие загрузки сцены
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Обновляем ссылку на игрока при загрузке новой сцены
        FindPlayerReference();
    }

    private void Start()
    {
        // Находим игрока по тегу
        FindPlayerReference();
        InitializeSlots();
        UpdateSelection();
    }

    private void Update()
    {
        HandleNumberInput();
        HandleMouseScroll();
        
        // Обработка выброса предмета по Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropSelectedItem();
        }

        // Обработка использования предмета по F
        if (Input.GetKeyDown(KeyCode.F))
        {
            UseSelectedItem();
        }
    }

    /// <summary>
    /// Поиск и обновление ссылки на игрока
    /// </summary>
    private void FindPlayerReference()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure there's an object with tag 'Player' in the scene.");
        }
    }

    void InitializeSlots()
    {
        // Автоматическое получение всех слотов в панели
        slots.Clear();
        foreach (Transform child in transform)
        {
            InventorySlot slot = child.GetComponent<InventorySlot>();
            if (slot != null)
            {
                slots.Add(slot);
            }
        }
    }

    void HandleNumberInput()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selectedSlotIndex = i;
                UpdateSelection();
                break;
            }
        }
    }

    void HandleMouseScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll > 0) // Прокрутка вверх
        {
            selectedSlotIndex--;
            if (selectedSlotIndex < 0)
                selectedSlotIndex = slots.Count - 1;
        }
        else if (scroll < 0) // Прокрутка вниз
        {
            selectedSlotIndex++;
            if (selectedSlotIndex >= slots.Count)
                selectedSlotIndex = 0;
        }

        UpdateSelection();
    }

    void UpdateSelection()
    {
        // Сбрасываем выделение всех слотов
        for (int i = 0; i < slots.Count; i++)
        {
            Image slotFrame = slots[i].GetComponent<Image>();
            if (slotFrame != null)
            {
                slotFrame.color = (i == selectedSlotIndex) ? selectedColor : normalColor;
            }
        }
    }

    void DropSelectedItem()
    {
        InventorySlot selectedSlot = GetSelectedSlot();
        
        // Если слот пустой, ничего не делаем
        if (selectedSlot.isEmpty)
            return;

        // Проверяем ссылку на игрока
        if (player == null)
        {
            Debug.LogWarning("Player reference is null! Attempting to find player...");
            FindPlayerReference();
            
            if (player == null)
            {
                Debug.LogError("Cannot drop item: Player reference is null!");
                return;
            }
        }

        // Выброс предмета из инвентаря - спавним префаб объекта перед персонажем
        Vector3 spawnPosition = player.position + Vector3.up + player.forward * 1.5f;

        // Для отладки
        Debug.Log("Player position: " + player.position);
        Debug.Log("Spawn position: " + spawnPosition);

        // Проверяем наличие 3D модели предмета
        if (selectedSlot.item == null || selectedSlot.item.item3D == null)
        {
            Debug.LogWarning("Cannot drop item: Item or its 3D model is null!");
            ClearSlotData(selectedSlot);
            return;
        }

        // Создаем предмет в мире
        GameObject itemObject = Instantiate(selectedSlot.item.item3D, spawnPosition, Quaternion.identity);
        
        // Устанавливаем количество объектов такое какое было в слоте
        Item worldItem = itemObject.GetComponent<Item>();
        if (worldItem != null)
        {
            worldItem.amount = selectedSlot.amount;
        }
        else
        {
            // Если у объекта нет компонента Item, добавляем его
            worldItem = itemObject.AddComponent<Item>();
            worldItem.item = selectedSlot.item;
            worldItem.amount = selectedSlot.amount;
        }

        // Очищаем слот
        ClearSlotData(selectedSlot);
    }

    void UseSelectedItem()
    {
        // Проверяем кд
        if (Time.time < lastUseTime + useCooldown)
            return;

        InventorySlot selectedSlot = GetSelectedSlot();
        
        // Если слот пустой, ничего не делаем
        if (selectedSlot.isEmpty || selectedSlot.item == null)
            return;

        // Вызываем функциональность предмета
        selectedSlot.item.Functionality();
        
        // Обновляем время последнего использования
        lastUseTime = Time.time;

        // Если предмет одноразовый (например, consumable), уменьшаем количество
        if (selectedSlot.item.isConsumable)
        {
            selectedSlot.amount--;
            if (selectedSlot.amount <= 0)
            {
                ClearSlotData(selectedSlot);
            }
            else
            {
                UpdateSlotDisplay(selectedSlot);
            }
        }
    }

    void ClearSlotData(InventorySlot slot)
    {
        // Убираем значения InventorySlot
        slot.item = null;
        slot.amount = 0;
        slot.isEmpty = true;
        
        Image iconImage = slot.itemIcon.GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.color = new Color(1, 1, 1, 0);
            iconImage.sprite = null;
        }
        
        if (slot.itemAmountText != null)
        {
            slot.itemAmountText.text = "";
        }
    }

    void UpdateSlotDisplay(InventorySlot slot)
    {
        // Обновляем отображение количества
        if (slot.amount > 1)
        {
            slot.itemAmountText.text = slot.amount.ToString();
        }
        else
        {
            slot.itemAmountText.text = "";
        }
    }

    // Метод для получения выбранного предмета
    public ItemScriptableObject GetSelectedItem()
    {
        return slots[selectedSlotIndex].item;
    }

    // Метод для получения выбранного слота
    public InventorySlot GetSelectedSlot()
    {
        return slots[selectedSlotIndex];
    }

    // Дополнительный метод для проверки, есть ли предмет в выбранном слоте
    public bool HasItemInSelectedSlot()
    {
        return !slots[selectedSlotIndex].isEmpty;
    }
}