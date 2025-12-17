using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class VendingMachineInteraction : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject vendingPanel;
    [SerializeField] private TextMeshProUGUI hasMoneyText;
    [SerializeField] private TextMeshProUGUI noMoneyText;
    [SerializeField] private TMP_InputField codeInputField;
    
    [Header("Vending Settings")]
    [SerializeField] private ItemScriptableObject vendingItem; 
    [SerializeField] private string correctCode = "42";
    
    [Header("Number Buttons")]
    [SerializeField] private Button[] numberButtons = new Button[10]; 
    [SerializeField] private Button cancelButton; 
    [SerializeField] private Button closeButton; 
    
    [Header("Raycast Settings")]
    [SerializeField] private float interactionRange = 5f;
    [SerializeField] private LayerMask interactionLayer;
    
    [Header("Input Settings")]
    [SerializeField] private InputAction rightMouseClick;
    
    private bool isLookingAtVendingMachine = false;
    private const int MAX_CODE_LENGTH = 2;
    private bool isProcessing = false; 

    // --- ДИНАМИЧЕСКИЕ СВОЙСТВА ВМЕСТО КЭШИРОВАНИЯ В START ---

    private InventoryManager CurrentInventory => InventoryManager.Instance;

    private GameObject CurrentHotbar
    {
        get
        {
            // Находим компонент HotbarPanel внутри живого синглтона UI
            if (PersistentObject.Instance != null)
            {
                var hp = PersistentObject.Instance.GetComponentInChildren<HotbarPanel>(true);
                return hp != null ? hp.gameObject : null;
            }
            return null;
        }
    }

    private Camera PlayerCamera => Camera.main;

    // -------------------------------------------------------

    private void Start()
    {
        rightMouseClick.Enable();
        rightMouseClick.performed += OnRightMouseClick;
        
        InitializeButtons();
        SetupInputField();
        
        if (hasMoneyText != null) hasMoneyText.gameObject.SetActive(false);
        if (noMoneyText != null) noMoneyText.gameObject.SetActive(false);
        if (vendingPanel != null) vendingPanel.SetActive(false);
    }
    
    private void SetupInputField()
    {
        if (codeInputField != null)
        {
            codeInputField.characterLimit = MAX_CODE_LENGTH;
            codeInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        }
    }
    
    private void InitializeButtons()
    {
        for (int i = 0; i < numberButtons.Length; i++)
        {
            if (numberButtons[i] != null)
            {
                int number = i;
                numberButtons[i].onClick.AddListener(() => AppendNumberToInput(number));
            }
        }
        
        if (cancelButton != null) cancelButton.onClick.AddListener(CancelInput);
        if (closeButton != null) closeButton.onClick.AddListener(CloseVendingPanel);
    }
    
    private void AppendNumberToInput(int number)
    {
        if (codeInputField != null && codeInputField.text.Length < MAX_CODE_LENGTH)
        {
            codeInputField.text += number.ToString();
            codeInputField.ActivateInputField();
        }
    }
    
    private void CancelInput()
    {
        if (codeInputField != null)
        {
            codeInputField.text = "";
            codeInputField.ActivateInputField();
        }
    }
    
    private void Update()
    {
        CheckVendingMachineRaycast();
        
        if (vendingPanel != null && vendingPanel.activeSelf)
        {
            HandleKeyboardInput();
            HandleEscapeInput();
        }
    }
    
    private void HandleKeyboardInput()
    {
        if (codeInputField == null) return;
        
        for (KeyCode key = KeyCode.Alpha0; key <= KeyCode.Alpha9; key++)
        {
            if (Input.GetKeyDown(key)) AppendNumberToInput((int)key - (int)KeyCode.Alpha0);
        }
        for (KeyCode key = KeyCode.Keypad0; key <= KeyCode.Keypad9; key++)
        {
            if (Input.GetKeyDown(key)) AppendNumberToInput((int)key - (int)KeyCode.Keypad0);
        }
    }
    
    private void HandleEscapeInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) CloseVendingPanel();
    }
    
    private void CheckVendingMachineRaycast()
    {
        Camera cam = PlayerCamera;
        if (cam == null || (vendingPanel != null && vendingPanel.activeSelf)) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactionLayer))
        {
            if (hit.collider.CompareTag("Vending"))
            {
                isLookingAtVendingMachine = true;
                UpdateVendingTextDisplay();
                return;
            }
        }
        
        isLookingAtVendingMachine = false;
        HideAllText();
    }
    
    private void UpdateVendingTextDisplay()
    {
        bool hasMoney = CheckForMoneyItem();
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
        InventoryManager inv = CurrentInventory;
        if (inv == null) return false;
        
        foreach (InventorySlot slot in inv.slots)
            if (slot.item != null && slot.item.itemType == ItemType.Money) return true;
        
        foreach (InventorySlot slot in inv.hotbarSlots)
            if (slot.item != null && slot.item.itemType == ItemType.Money) return true;
        
        return false;
    }
    
    private bool RemoveMoneyItem()
    {
        InventoryManager inv = CurrentInventory;
        if (inv == null) return false;
        
        // Сначала ищем в инвентаре, потом в хотбаре
        List<InventorySlot> allSlots = new List<InventorySlot>(inv.slots);
        allSlots.AddRange(inv.hotbarSlots);

        foreach (InventorySlot slot in allSlots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Money && slot.amount > 0)
            {
                slot.amount--;
                if (slot.amount <= 0) slot.ClearSlot(); // Используем метод ClearSlot, который мы добавили ранее
                else if (slot.itemAmountText != null) slot.itemAmountText.text = slot.amount > 1 ? slot.amount.ToString() : "";
                
                return true;
            }
        }
        return false;
    }
    
    private bool AddVendingItemToInventory()
    {
        InventoryManager inv = CurrentInventory;
        if (inv == null || vendingItem == null) return false;
        
        // Используем метод из InventoryManager, если он публичный, 
        // или реализуем локально через те же слоты
        return AddItemToFirstAvailableSlot(inv.hotbarSlots, vendingItem, 1) || 
               AddItemToFirstAvailableSlot(inv.slots, vendingItem, 1);
    }
    
    private bool AddItemToFirstAvailableSlot(List<InventorySlot> targetSlots, ItemScriptableObject item, int amount)
    {
        foreach (InventorySlot slot in targetSlots)
        {
            if (slot.isEmpty)
            {
                slot.item = item;
                slot.amount = amount;
                slot.isEmpty = false;
                slot.SetIcon(item.icon);
                if (slot.itemAmountText != null) slot.itemAmountText.text = amount > 1 ? amount.ToString() : "";
                return true;
            }
        }
        return false;
    }
    
    private void OnRightMouseClick(InputAction.CallbackContext context)
    {
        if (!isLookingAtVendingMachine || isProcessing) return;
        
        if (CheckForMoneyItem())
        {
            if (vendingPanel != null)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                vendingPanel.SetActive(true);
                
                GameObject hb = CurrentHotbar;
                if (hb != null) hb.SetActive(false);
                
                CancelInput();
                HideAllText();
            }
        }
    }
    
    public void CloseVendingPanel()
    {
        if (vendingPanel != null)
        {
            vendingPanel.SetActive(false);
            
            GameObject hb = CurrentHotbar;
            if (hb != null) hb.SetActive(true);
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isProcessing = false;
        }
    }
    
    public void OnApplyButtonClicked()
    {
        if (isProcessing) return;
        if (codeInputField == null || string.IsNullOrEmpty(codeInputField.text)) return;
        
        isProcessing = true;
        
        if (codeInputField.text == correctCode)
        {
            if (!CheckForMoneyItem())
            {
                StartCoroutine(ShowErrorMessage("НЕТ ДЕНЕГ"));
                return;
            }
            
            if (RemoveMoneyItem() && AddVendingItemToInventory())
            {
                CloseVendingPanel();
            }
            else
            {
                StartCoroutine(ShowErrorMessage("НЕТ МЕСТА"));
            }
        }
        else
        {
            StartCoroutine(ShowErrorMessage("ОШИБКА"));
        }
    }
    
    private IEnumerator ShowErrorMessage(string message)
    {
        if (codeInputField == null) yield break;
        codeInputField.text = message;
        codeInputField.interactable = false;
        SetButtonsInteractable(false);
        
        yield return new WaitForSeconds(1.5f);
        
        codeInputField.text = "";
        codeInputField.interactable = true;
        SetButtonsInteractable(true);
        isProcessing = false;
    }
    
    private void SetButtonsInteractable(bool interactable)
    {
        foreach (Button b in numberButtons) if (b != null) b.interactable = interactable;
        if (cancelButton != null) cancelButton.interactable = interactable;
    }
    
    private void OnDestroy()
    {
        rightMouseClick.performed -= OnRightMouseClick;
        rightMouseClick.Disable();
    }
}