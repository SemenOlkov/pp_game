using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Обработчик перетаскивания предметов в инвентаре
/// IPointerDownHandler - Следит за нажатиями мышки по объекту
/// IPointerUpHandler - Следит за отпусканием мышки по объекту
/// IDragHandler - Следит за перетаскиванием мышки по объекту
/// </summary>
public class DragAndDropItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public InventorySlot oldSlot;
    private Transform player;
    
    private RectTransform rectTransform;
    private Image itemImage;
    private bool isDragging = false;
    
    private void Start()
    {
        // Получаем компоненты
        rectTransform = GetComponent<RectTransform>();
        itemImage = GetComponentInChildren<Image>();
        
        // Находим скрипт InventorySlot в родителе
        oldSlot = GetComponentInParent<InventorySlot>();
        
        // Получаем ссылку на игрока через менеджер инвентаря
        FindPlayerReference();
    }
    
    /// <summary>
    /// Публичный метод для обновления ссылки на игрока
    /// </summary>
    public void UpdatePlayerReference()
    {
        FindPlayerReference();
    }
    
    /// <summary>
    /// Поиск ссылки на игрока
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
    
    public void OnDrag(PointerEventData eventData)
    {
        // Если слот пустой или не перетаскиваем, то выходим
        if (oldSlot.isEmpty || !isDragging)
            return;
            
        // Обновляем позицию перетаскиваемого объекта
        rectTransform.position += new Vector3(eventData.delta.x, eventData.delta.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Если слот пустой, то выходим
        if (oldSlot.isEmpty)
            return;
            
        isDragging = true;
        
        // Делаем картинку прозрачнее
        itemImage.color = new Color(1, 1, 1, 0.75f);
        
        // Отключаем raycast, чтобы клики проходили сквозь перетаскиваемый объект
        itemImage.raycastTarget = false;
        
        // Перемещаем объект на верхний уровень, чтобы он был поверх других элементов UI
        transform.SetParent(transform.parent.parent.parent);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging)
            return;
            
        isDragging = false;
        
        // Возвращаем непрозрачность картинки
        itemImage.color = new Color(1, 1, 1, 1f);
        
        // Включаем raycast обратно
        itemImage.raycastTarget = true;
        
        // Возвращаем объект в исходный слот
        transform.SetParent(oldSlot.transform);
        transform.localPosition = Vector3.zero;
        
        // Обработка события отпускания
        HandleDrop(eventData);
    }
    
    /// <summary>
    /// Обработка события отпускания предмета
    /// </summary>
    private void HandleDrop(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;
            
        // Проверяем, был ли отпущен предмет над UI фоновым элементом
        if (eventData.pointerCurrentRaycast.gameObject != null && 
            eventData.pointerCurrentRaycast.gameObject.name == "UIBG")
        {
            // Выбрасываем предмет из инвентаря
            DropItemFromInventory();
        }
        // Проверяем, был ли отпущен над другим слотом инвентаря
        else if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            // Получаем слот, над которым отпустили предмет
            InventorySlot newSlot = GetSlotUnderPointer(eventData);
            
            if (newSlot != null && newSlot != oldSlot)
            {
                // Перемещаем предмет между слотами
                ExchangeSlotData(newSlot);
            }
        }
    }
    
    /// <summary>
    /// Выбрасывание предмета из инвентаря в мир
    /// </summary>
    private void DropItemFromInventory()
    {
        // Проверяем ссылку на игрока
        if (player == null)
        {
            Debug.LogWarning("Cannot drop item: Player reference is null!");
            return;
        }
        
        // Проверяем, есть ли у предмета 3D модель
        if (oldSlot.item.item3D == null)
        {
            Debug.LogWarning($"Cannot drop {oldSlot.item.itemName}: No 3D model assigned!");
            ClearSlotData();
            return;
        }
        
        // Вычисляем позицию спавна перед игроком
        Vector3 spawnPosition = player.position + 
                               Vector3.up + 
                               player.forward * 1.5f;
        
        Debug.Log($"Dropping item: {oldSlot.item.itemName} at position: {spawnPosition}");
        
        try
        {
            // Создаем объект в мире
            GameObject itemObject = Instantiate(oldSlot.item.item3D, spawnPosition, Quaternion.identity);
            
            // Устанавливаем количество предметов
            Item worldItem = itemObject.GetComponent<Item>();
            if (worldItem != null)
            {
                worldItem.amount = oldSlot.amount;
            }
            else
            {
                // Если у объекта нет компонента Item, добавляем его
                worldItem = itemObject.AddComponent<Item>();
                worldItem.item = oldSlot.item;
                worldItem.amount = oldSlot.amount;
            }
            
            // Очищаем слот
            ClearSlotData();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error dropping item: {e.Message}");
        }
    }
    
    /// <summary>
    /// Получение слота под курсором
    /// </summary>
    private InventorySlot GetSlotUnderPointer(PointerEventData eventData)
    {
        GameObject target = eventData.pointerCurrentRaycast.gameObject;
        
        // Поднимаемся по иерархии, чтобы найти InventorySlot
        while (target != null)
        {
            InventorySlot slot = target.GetComponent<InventorySlot>();
            if (slot != null)
                return slot;
                
            target = target.transform.parent != null ? target.transform.parent.gameObject : null;
        }
        
        return null;
    }
    
    /// <summary>
    /// Очистка данных слота
    /// </summary>
    private void ClearSlotData()
    {
        if (oldSlot == null)
            return;
            
        oldSlot.item = null;
        oldSlot.amount = 0;
        oldSlot.isEmpty = true;
        
        Image iconImage = oldSlot.itemIcon.GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.color = new Color(1, 1, 1, 0);
            iconImage.sprite = null;
        }
        
        if (oldSlot.itemAmountText != null)
        {
            oldSlot.itemAmountText.text = "";
        }
    }
    
    /// <summary>
    /// Обмен данными между слотами
    /// </summary>
    private void ExchangeSlotData(InventorySlot newSlot)
    {
        // Проверяем валидность слотов
        if (oldSlot == null || newSlot == null)
            return;
            
        // Проверяем, можно ли объединить стаки
        if (oldSlot.item == newSlot.item && 
            oldSlot.item != null && 
            newSlot.item != null)
        {
            // Объединяем стаки
            int totalAmount = oldSlot.amount + newSlot.amount;
            int maxAmount = oldSlot.item.maximumAmount;
            
            if (totalAmount <= maxAmount)
            {
                newSlot.amount = totalAmount;
                UpdateSlotUI(newSlot, totalAmount);
                ClearSlotData();
                return;
            }
            else
            {
                // Если превышает максимум, заполняем до максимума
                int remaining = maxAmount - newSlot.amount;
                newSlot.amount = maxAmount;
                oldSlot.amount -= remaining;
                
                UpdateSlotUI(newSlot, newSlot.amount);
                UpdateSlotUI(oldSlot, oldSlot.amount);
                return;
            }
        }
        
        // Временно сохраняем данные нового слота
        ItemScriptableObject tempItem = newSlot.item;
        int tempAmount = newSlot.amount;
        bool tempIsEmpty = newSlot.isEmpty;
        
        // Копируем иконку и текст для восстановления
        Image newSlotImage = newSlot.itemIcon.GetComponent<Image>();
        Sprite newSlotSprite = newSlotImage.sprite;
        Color newSlotColor = newSlotImage.color;
        string newSlotText = newSlot.itemAmountText != null ? newSlot.itemAmountText.text : "";
        
        // Заменяем данные нового слота данными старого слота
        newSlot.item = oldSlot.item;
        newSlot.amount = oldSlot.amount;
        newSlot.isEmpty = oldSlot.isEmpty;
        
        // Обновляем UI нового слота
        if (!oldSlot.isEmpty)
        {
            newSlot.SetIcon(oldSlot.itemIcon.GetComponent<Image>().sprite);
            newSlot.itemAmountText.text = oldSlot.amount > 1 ? oldSlot.amount.ToString() : "";
        }
        else
        {
            newSlotImage.color = new Color(1, 1, 1, 0);
            newSlotImage.sprite = null;
            if (newSlot.itemAmountText != null)
                newSlot.itemAmountText.text = "";
        }
        
        // Заменяем данные старого слота сохраненными данными
        oldSlot.item = tempItem;
        oldSlot.amount = tempAmount;
        oldSlot.isEmpty = tempIsEmpty;
        
        // Обновляем UI старого слота
        if (!tempIsEmpty)
        {
            oldSlot.SetIcon(newSlotSprite);
            if (oldSlot.itemAmountText != null)
                oldSlot.itemAmountText.text = tempAmount > 1 ? tempAmount.ToString() : "";
        }
        else
        {
            Image oldSlotImage = oldSlot.itemIcon.GetComponent<Image>();
            oldSlotImage.color = new Color(1, 1, 1, 0);
            oldSlotImage.sprite = null;
            if (oldSlot.itemAmountText != null)
                oldSlot.itemAmountText.text = "";
        }
    }
    
    /// <summary>
    /// Обновление UI слота
    /// </summary>
    private void UpdateSlotUI(InventorySlot slot, int amount)
    {
        if (slot == null)
            return;
            
        if (slot.itemAmountText != null)
        {
            slot.itemAmountText.text = amount > 1 ? amount.ToString() : "";
        }
    }
    
    private void OnDisable()
    {
        // Если отключаем объект во время перетаскивания, сбрасываем состояние
        if (isDragging)
        {
            ResetDragState();
        }
    }
    
    /// <summary>
    /// Сброс состояния перетаскивания
    /// </summary>
    private void ResetDragState()
    {
        isDragging = false;
        
        if (itemImage != null)
        {
            itemImage.color = new Color(1, 1, 1, 1f);
            itemImage.raycastTarget = true;
        }
        
        if (oldSlot != null)
        {
            transform.SetParent(oldSlot.transform);
            transform.localPosition = Vector3.zero;
        }
    }
}