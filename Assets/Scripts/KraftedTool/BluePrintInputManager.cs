using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

public class BlueprintInputManager : MonoBehaviour
{
    [Header("Blueprint References")]
    public GameObject blueprintUI;
    public GameObject hotbarPanel;
    
    private Keyboard keyboard;
    private BlueprintItem currentBlueprintItem;
    private BlueprintPagesManager pagesManager;
    public GameObject craftHint;
    public GameObject notCraftHint;
    public GameObject alreadyCraftedHint;
    public bool isCraftable = false;
    public bool isCrafted = false;
    
    [Header("Inventory Reference")]
    public InventoryManager inventoryManager;
    
    [Header("Crafting Result")]
    public ItemScriptableObject craftedItem; // Предмет, который получается после крафта

    private void Start()
    {
        keyboard = Keyboard.current;
        
        // Получаем компонент менеджера страниц
        pagesManager = GetComponentInChildren<BlueprintPagesManager>(true);
        if (pagesManager == null)
        {
            pagesManager = blueprintUI.GetComponentInChildren<BlueprintPagesManager>(true);
        }
        
        // Изначально скрываем UI
        if (blueprintUI != null)
            blueprintUI.SetActive(false);
            
        // Если inventoryManager не присвоен в инспекторе, пытаемся найти его
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }
    }

    private void Update()
    {
        if (blueprintUI != null && blueprintUI.activeSelf && keyboard != null && keyboard.fKey.wasPressedThisFrame)
        {
            CloseBlueprintUI();
        }
        
        if (blueprintUI != null && blueprintUI.activeSelf && keyboard != null && keyboard.cKey.wasPressedThisFrame && isCraftable)
        {
            Craft();
        }
        
        // Проверяем инвентарь на наличие нужных предметов
        CheckCraftingMaterials();
        
        craftHint.SetActive(isCraftable);
        notCraftHint.SetActive(!isCraftable && !isCrafted);
        alreadyCraftedHint.SetActive(isCrafted);
    }
    
    private void CheckCraftingMaterials()
    {
        if (inventoryManager == null)
            return;
            
        int totalPartItems = 0;
        
        // Проверяем основные слоты инвентаря
        foreach (InventorySlot slot in inventoryManager.slots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Part)
            {
                totalPartItems += slot.amount;
            }
        }
        
        // Проверяем слоты горячей панели
        foreach (InventorySlot slot in inventoryManager.hotbarSlots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Part)
            {
                totalPartItems += slot.amount;
            }
        }
        
        // Устанавливаем флаг craftability
        isCraftable = (totalPartItems >= 4);
    }
    
    private void Craft()
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager is not assigned!");
            return;
        }
        
        if (craftedItem == null)
        {
            Debug.LogError("Crafted item is not assigned in BlueprintInputManager!");
            return;
        }
        
        // Логируем начало крафта
        Debug.Log("Crafting started... Removing all Part items from inventory.");
        
        // Удаляем Part предметы из слотов горячей панели
        foreach (InventorySlot slot in inventoryManager.hotbarSlots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Part)
            {
                Debug.Log($"Removing {slot.amount} {slot.item.itemName} from hotbar slot");
                ClearSlot(slot);
            }
        }
        
        // Удаляем Part предметы из основных слотов инвентаря
        foreach (InventorySlot slot in inventoryManager.slots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Part)
            {
                Debug.Log($"Removing {slot.amount} {slot.item.itemName} from inventory slot");
                ClearSlot(slot);
            }
        }
        
        // Добавляем созданный предмет в инвентарь
        AddCraftedItemToInventory();
        
        // Обновляем флаг craftable
        CheckCraftingMaterials();
        isCrafted = true;
        Debug.Log("Crafting completed! Item added to inventory.");
    }
    
    private void ClearSlot(InventorySlot slot)
    {
        if (slot == null) return;
        
        // Очищаем слот
        slot.item = null;
        slot.amount = 0;
        slot.isEmpty = true;
        
        // Скрываем иконку и очищаем текст
        if (slot.itemIcon != null)
        {
            // Получаем компонент Image у иконки
            Image iconImage = slot.itemIcon.GetComponent<Image>();
            if (iconImage != null)
            {
                // Делаем иконку прозрачной и убираем спрайт
                iconImage.color = new Color(1, 1, 1, 0);
                iconImage.sprite = null;
            }
        }
        
        if (slot.itemAmountText != null)
            slot.itemAmountText.text = "";
    }
    
    private void AddCraftedItemToInventory()
    {
        if (craftedItem == null)
            return;
            
        // Пытаемся добавить в хотбар (для удобства использования)
        bool added = AddItemToFirstAvailableSlot(inventoryManager.hotbarSlots, craftedItem, 1);
        
        // Если не удалось добавить в хотбар, пробуем в основной инвентарь
        if (!added)
        {
            added = AddItemToFirstAvailableSlot(inventoryManager.slots, craftedItem, 1);
        }
        
        if (!added)
        {
            Debug.LogWarning("No available slots in inventory for crafted item!");
        }
        else
        {
            Debug.Log($"Successfully added {craftedItem.itemName} to inventory.");
        }
    }
    
    private bool AddItemToFirstAvailableSlot(List<InventorySlot> slots, ItemScriptableObject item, int amount)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.isEmpty)
            {
                // Нашли пустой слот, добавляем предмет
                slot.item = item;
                slot.amount = amount;
                slot.isEmpty = false;
                
                // Устанавливаем иконку
                if (slot.itemIcon != null)
                {
                    Image iconImage = slot.itemIcon.GetComponent<Image>();
                    if (iconImage != null)
                    {
                        iconImage.color = new Color(1, 1, 1, 1);
                        iconImage.sprite = item.icon;
                    }
                }
                
                // Устанавливаем текст количества
                if (slot.itemAmountText != null)
                {
                    slot.itemAmountText.text = amount > 1 ? amount.ToString() : "";
                }
                
                return true;
            }
        }
        
        return false;
    }

    public void OpenBlueprintUI(BlueprintItem blueprintItem)
    {
        currentBlueprintItem = blueprintItem;
        
        if (blueprintUI != null)
        {
            blueprintUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if (hotbarPanel != null)
                hotbarPanel.SetActive(false);
            
            // Передаем страницы в менеджер пагинации
            if (pagesManager != null && blueprintItem.blueprintPages != null)
            {
                pagesManager.SetPages(blueprintItem.blueprintPages);
            }
            else
            {
                Debug.LogError("BlueprintPagesManager not found or no pages in blueprint!");
            }
            
            // Проверяем материалы при открытии UI
            CheckCraftingMaterials();
        }
    }

    private void CloseBlueprintUI()
    {
        if (blueprintUI != null)
        {
            blueprintUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (hotbarPanel != null)
                hotbarPanel.SetActive(true);
        }
    }
}