using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class VendingMachineInteraction : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject vendingPanel;
    [SerializeField] private TextMeshProUGUI hasMoneyText;
    [SerializeField] private TextMeshProUGUI noMoneyText;
    
    [Header("Raycast Settings")]
    [SerializeField] private float interactionRange = 5f;

    
    [Header("Input Settings")]
    [SerializeField] private InputAction rightMouseClick;
    
    private Camera playerCamera;
    private InventoryManager inventoryManager;
    private bool isLookingAtVendingMachine = false;
    
    private void Start()
    {
        playerCamera = Camera.main;
        
        // Находим InventoryManager
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found in scene!");
        }
        
        // Настраиваем Input Action
        rightMouseClick.Enable();
        rightMouseClick.performed += OnRightMouseClick;
        
        // Скрываем все UI элементы при старте
        if (hasMoneyText != null) hasMoneyText.gameObject.SetActive(false);
        if (noMoneyText != null) noMoneyText.gameObject.SetActive(false);
        if (vendingPanel != null) vendingPanel.SetActive(false);
    }
    
    private void Update()
    {
        CheckVendingMachineRaycast();
    }
    
    private void CheckVendingMachineRaycast()
    {
        if (playerCamera == null) return;
        
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            if (hit.collider.CompareTag("Vending"))
            {
                isLookingAtVendingMachine = true;
                UpdateVendingTextDisplay();
                return;
            }
        }
        
        // Если не смотрим на вендинговый аппарат
        isLookingAtVendingMachine = false;
        HideAllText();
    }
    
    private void UpdateVendingTextDisplay()
    {
        if (inventoryManager == null) return;
        
        bool hasMoney = CheckForMoneyItem();
        
        // Показываем соответствующий текст
        if (hasMoneyText != null) hasMoneyText.gameObject.SetActive(hasMoney);
        if (noMoneyText != null) noMoneyText.gameObject.SetActive(!hasMoney);
    }
    
    private void HideAllText()
    {
        if (hasMoneyText != null) hasMoneyText.gameObject.SetActive(false);
        if (noMoneyText != null) noMoneyText.gameObject.SetActive(false);
    }
    
    private bool CheckForMoneyItem()
    {
        if (inventoryManager == null) return false;
        
        // Проверяем основные слоты инвентаря
        foreach (InventorySlot slot in inventoryManager.slots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Money)
            {
                return true;
            }
        }
        
        // Проверяем слоты горячей панели
        foreach (InventorySlot slot in inventoryManager.hotbarSlots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Money)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private void OnRightMouseClick(InputAction.CallbackContext context)
    {
        if (!isLookingAtVendingMachine) return;
        
        if (CheckForMoneyItem())
        {
            // Открываем панель вендингового аппарата
            if (vendingPanel != null)
            {
                vendingPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                
                // Скрываем текст
                HideAllText();
            }
        }
    }
    
    public void CloseVendingPanel()
    {
        if (vendingPanel != null)
        {
            vendingPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    private void OnDestroy()
    {
        // Отключаем Input Action при уничтожении объекта
        rightMouseClick.performed -= OnRightMouseClick;
        rightMouseClick.Disable();
    }
}