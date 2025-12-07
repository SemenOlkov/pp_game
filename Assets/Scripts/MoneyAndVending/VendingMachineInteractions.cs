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
    [SerializeField] private ItemScriptableObject vendingItem; // Предмет, который выдается за код 42
    [SerializeField] private string correctCode = "42";
    
    [Header("Number Buttons")]
    [SerializeField] private Button[] numberButtons = new Button[10]; // 0-9
    [SerializeField] private Button cancelButton; // Стирает введенный текст
    [SerializeField] private Button closeButton; // Закрывает всю панель
    
    [Header("Raycast Settings")]
    [SerializeField] private float interactionRange = 5f;
    [SerializeField] private LayerMask interactionLayer;
    
    [Header("Input Settings")]
    [SerializeField] private InputAction rightMouseClick;
    
    private Camera playerCamera;
    private InventoryManager inventoryManager;
    public GameObject hotbarPanel;
    private bool isLookingAtVendingMachine = false;
    private const int MAX_CODE_LENGTH = 2;
    private bool isProcessing = false; // Флаг, чтобы избежать множественных нажатий
    
    private void Start()
    {
        playerCamera = Camera.main;
        
        // Находим InventoryManager и HotbarPanel
        inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found in scene!");
        }
        
        // Настраиваем Input Action
        rightMouseClick.Enable();
        rightMouseClick.performed += OnRightMouseClick;
        
        // Инициализируем кнопки
        InitializeButtons();
        
        // Настраиваем поле ввода
        SetupInputField();
        
        // Скрываем все UI элементы при старте
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
            codeInputField.onValueChanged.AddListener(OnInputValueChanged);
        }
    }
    
    private void OnInputValueChanged(string newText)
    {
        // Логика при изменении текста
    }
    
    private void InitializeButtons()
    {
        // Настраиваем цифровые кнопки
        for (int i = 0; i < numberButtons.Length; i++)
        {
            if (numberButtons[i] != null)
            {
                int number = i;
                numberButtons[i].onClick.AddListener(() => AppendNumberToInput(number));
            }
        }
        
        // Настраиваем кнопку отмены
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(CancelInput);
        }
        
        // Настраиваем кнопку закрытия
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseVendingPanel);
        }
    }
    
    private void AppendNumberToInput(int number)
    {
        if (codeInputField != null && codeInputField.text.Length < MAX_CODE_LENGTH)
        {
            codeInputField.text += number.ToString();
            codeInputField.Select();
            codeInputField.ActivateInputField();
        }
    }
    
    private void CancelInput()
    {
        if (codeInputField != null)
        {
            codeInputField.text = "";
            codeInputField.Select();
            codeInputField.ActivateInputField();
        }
    }
    
    private void Update()
    {
        CheckVendingMachineRaycast();
        
        if (vendingPanel.activeSelf)
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
            if (Input.GetKeyDown(key))
            {
                int number = (int)key - (int)KeyCode.Alpha0;
                AppendNumberToInput(number);
            }
        }
        
        for (KeyCode key = KeyCode.Keypad0; key <= KeyCode.Keypad9; key++)
        {
            if (Input.GetKeyDown(key))
            {
                int number = (int)key - (int)KeyCode.Keypad0;
                AppendNumberToInput(number);
            }
        }
    }
    
    private void HandleEscapeInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseVendingPanel();
        }
    }
    
    private void CheckVendingMachineRaycast()
    {
        if (playerCamera == null) return;
        if (vendingPanel.activeSelf){
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, interactionRange, interactionLayer))
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
        if (inventoryManager == null) return;
        
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
    
    private bool RemoveMoneyItem()
    {
        if (inventoryManager == null) return false;
        
        // Ищем и удаляем один предмет типа Money из горячей панели
        foreach (InventorySlot slot in inventoryManager.hotbarSlots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Money && slot.amount > 0)
            {
                slot.amount--;
                if (slot.itemAmountText != null)
                {
                    slot.itemAmountText.text = slot.amount > 1 ? slot.amount.ToString() : "";
                }
                
                // Если предметов больше нет, очищаем слот
                if (slot.amount <= 0)
                {
                    slot.item = null;
                    slot.amount = 0;
                    slot.isEmpty = true;
                    
                    // Очищаем иконку
                    if (slot.itemIcon != null)
                    {
                        Image iconImage = slot.itemIcon.GetComponent<Image>();
                        if (iconImage != null)
                        {
                            iconImage.color = new Color(1, 1, 1, 0);
                            iconImage.sprite = null;
                        }
                    }
                    
                    if (slot.itemAmountText != null)
                        slot.itemAmountText.text = "";
                }
                
                Debug.Log("Деньги списаны!");
                return true;
            }
        }
        
        // Если не нашли в горячей панели, ищем в основном инвентаре
        foreach (InventorySlot slot in inventoryManager.slots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Money && slot.amount > 0)
            {
                slot.amount--;
                if (slot.itemAmountText != null)
                {
                    slot.itemAmountText.text = slot.amount > 1 ? slot.amount.ToString() : "";
                }
                
                // Если предметов больше нет, очищаем слот
                if (slot.amount <= 0)
                {
                    slot.item = null;
                    slot.amount = 0;
                    slot.isEmpty = true;
                    
                    // Очищаем иконку
                    if (slot.itemIcon != null)
                    {
                        Image iconImage = slot.itemIcon.GetComponent<Image>();
                        if (iconImage != null)
                        {
                            iconImage.color = new Color(1, 1, 1, 0);
                            iconImage.sprite = null;
                        }
                    }
                    
                    if (slot.itemAmountText != null)
                        slot.itemAmountText.text = "";
                }
                
                Debug.Log("Деньги списаны!");
                return true;
            }
        }
        
        Debug.Log("Не удалось списать деньги!");
        return false;
    }
    
    private bool AddVendingItemToInventory()
    {
        if (inventoryManager == null || vendingItem == null) return false;
        
        // Пытаемся добавить предмет в инвентарь
        bool added = AddItemToFirstAvailableSlot(inventoryManager.hotbarSlots, vendingItem, 1);
        
        if (!added)
        {
            added = AddItemToFirstAvailableSlot(inventoryManager.slots, vendingItem, 1);
        }
        
        if (added)
        {
            Debug.Log($"Предмет {vendingItem.itemName} добавлен в инвентарь!");
        }
        else
        {
            Debug.Log("Не удалось добавить предмет в инвентарь!");
        }
        
        return added;
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
    
    private void OnRightMouseClick(InputAction.CallbackContext context)
    {
        if (!isLookingAtVendingMachine || isProcessing) return;
        
        if (CheckForMoneyItem())
        {
            if (vendingPanel != null)
            {
                vendingPanel.SetActive(true);
                hotbarPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                
                CancelInput();
                HideAllText();
                
                if (codeInputField != null)
                {
                    codeInputField.Select();
                    codeInputField.ActivateInputField();
                }
            }
        }
    }
    
    public void CloseVendingPanel()
    {
        if (vendingPanel != null)
        {
            vendingPanel.SetActive(false);
            hotbarPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isProcessing = false;
        }
    }
    
    public void OnApplyButtonClicked()
    {
        if (isProcessing) return;
        
        if (codeInputField == null || string.IsNullOrEmpty(codeInputField.text))
        {
            Debug.Log("Введите код!");
            return;
        }
        
        isProcessing = true;
        
        if (codeInputField.text == correctCode)
        {
            // Проверяем, есть ли у игрока деньги
            if (!CheckForMoneyItem())
            {
                StartCoroutine(ShowErrorMessage("Недостаточно денег!"));
                isProcessing = false;
                return;
            }
            
            // Списываем деньги
            if (!RemoveMoneyItem())
            {
                StartCoroutine(ShowErrorMessage("Ошибка списания денег!"));
                isProcessing = false;
                return;
            }
            
            // Добавляем предмет
            if (AddVendingItemToInventory())
            {
                Debug.Log("Товар успешно выдан!");
                // Закрываем панель после успешной покупки
                CloseVendingPanel();
            }
            else
            {
                StartCoroutine(ShowErrorMessage("Не удалось выдать товар!"));
            }
        }
        else
        {
            // Неправильный код
            StartCoroutine(ShowErrorMessage("Ошибка!"));
        }
    }
    
    private IEnumerator ShowErrorMessage(string message)
    {
        if (codeInputField == null) yield break;
        
        // Сохраняем оригинальный текст (хотя он должен быть кодом)
        string originalText = codeInputField.text;
        
        // Показываем сообщение об ошибке
        codeInputField.text = message;
        codeInputField.interactable = false;
        
        // Блокируем кнопки на время показа сообщения
        SetButtonsInteractable(false);
        
        // Ждем 2 секунды
        yield return new WaitForSeconds(2f);
        
        // Восстанавливаем поле ввода
        codeInputField.text = "";
        codeInputField.interactable = true;
        codeInputField.Select();
        codeInputField.ActivateInputField();
        
        // Разблокируем кнопки
        SetButtonsInteractable(true);
        
        isProcessing = false;
    }
    
    private void SetButtonsInteractable(bool interactable)
    {
        foreach (Button button in numberButtons)
        {
            if (button != null)
            {
                button.interactable = interactable;
            }
        }
        
        if (cancelButton != null)
        {
            cancelButton.interactable = interactable;
        }
    }
    
    private void OnDestroy()
    {
        rightMouseClick.performed -= OnRightMouseClick;
        rightMouseClick.Disable();
        
        foreach (Button button in numberButtons)
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
            }
        }
        
        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
        }
        
        if (codeInputField != null)
        {
            codeInputField.onValueChanged.RemoveAllListeners();
        }
    }
}