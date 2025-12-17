using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    public GameObject UIBG;
    public Transform inventoryPanel;
    public Transform hotbarPanel;
    public List<InventorySlot> slots = new List<InventorySlot>();
    public List<InventorySlot> hotbarSlots = new List<InventorySlot>();
    public bool isOpened;
    
    private Keyboard keyboard;
    private Mouse mouse;
    private Camera mainCamera;
    public float reachDistance = 10f;
    public GameObject crosshair;
    [SerializeField] private LayerMask interactionLayer;
    
    // Добавлены новые переменные
    public GameObject itemPickupCanvas;
    
    // Синглтон паттерн
    public static InventoryManager Instance { get; private set; }
    
    // Флаг для проверки необходимости поиска камеры
    private bool cameraNeedsUpdate = false;
    
    private void Awake()
    {
        Instance = this; // Простое присваивание, так как он внутри PersistentObject
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // При загрузке новой сцены помечаем, что нужно обновить камеру
        cameraNeedsUpdate = true;
        
        // Также обновляем ссылку на игрока для DragAndDropItem
        UpdatePlayerReference();
    }
    
    void Start()
    {   
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Находим камеру при старте
        FindMainCamera();
        
        UIBG.SetActive(false);
        inventoryPanel.gameObject.SetActive(false);
        if (itemPickupCanvas != null)
            itemPickupCanvas.SetActive(false);
        
        for(int i = 0; i < inventoryPanel.childCount; i++)
        {
            if(inventoryPanel.GetChild(i).GetComponent<InventorySlot>() != null)
            {
                slots.Add(inventoryPanel.GetChild(i).GetComponent<InventorySlot>());
            }
        }
        for(int i = 0; i < hotbarPanel.childCount; i++)
        {
            if(hotbarPanel.GetChild(i).GetComponent<InventorySlot>() != null)
            {
                hotbarSlots.Add(hotbarPanel.GetChild(i).GetComponent<InventorySlot>());
            }
        }
        
        keyboard = Keyboard.current;
        mouse = Mouse.current;
    }
    
    void Update() 
    {
        // Проверяем, нужно ли обновить камеру
        if (cameraNeedsUpdate || mainCamera == null)
        {
            FindMainCamera();
            cameraNeedsUpdate = false;
        }
        
        if (keyboard != null && keyboard.eKey.wasPressedThisFrame)
        {
            if (isOpened)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
            isOpened = !isOpened;
        }
        
        // Если камера всё ещё null, выходим из метода
        if (mainCamera == null)
        {
            Debug.LogWarning("Camera not found! Raycast functionality disabled.");
            return;
        }
        
        // Остальной код Update...
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        bool isUIBGActive = UIBG.activeInHierarchy;

        if(!isUIBGActive && Physics.Raycast(ray, out hit, reachDistance, interactionLayer))
        {
            if(hit.collider.gameObject.GetComponent<Item>() != null)
            {
                if (itemPickupCanvas != null)
                    itemPickupCanvas.SetActive(true);
                
                if(mouse != null && mouse.leftButton.wasPressedThisFrame)
                {
                    AddItem(hit.collider.gameObject.GetComponent<Item>().item, hit.collider.gameObject.GetComponent<Item>().amount);
                    Destroy(hit.collider.gameObject);
                    if (itemPickupCanvas != null)
                        itemPickupCanvas.SetActive(false);
                }
            }
            else
            {
                if (itemPickupCanvas != null)
                    itemPickupCanvas.SetActive(false);
            }
        }
        else
        {
            if (itemPickupCanvas != null)
                itemPickupCanvas.SetActive(false);
        }
    }
    
    // Метод для поиска главной камеры
    private void FindMainCamera()
    {
        mainCamera = Camera.main;
        
        if (mainCamera == null)
        {
            // Ищем камеру вручную, если Camera.main не находит
            Camera[] cameras = FindObjectsOfType<Camera>();
            foreach (Camera cam in cameras)
            {
                if (cam.gameObject.tag == "MainCamera" || cam.gameObject.name.Contains("Main Camera"))
                {
                    mainCamera = cam;
                    Debug.Log("Found camera manually: " + mainCamera.name);
                    break;
                }
            }
            
            if (mainCamera == null && cameras.Length > 0)
            {
                // Берём первую активную камеру
                foreach (Camera cam in cameras)
                {
                    if (cam.isActiveAndEnabled)
                    {
                        mainCamera = cam;
                        Debug.Log("Using first active camera: " + mainCamera.name);
                        break;
                    }
                }
            }
        }
        
        if (mainCamera != null)
        {
            Debug.Log("Camera found: " + mainCamera.name);
        }
        else
        {
            Debug.LogError("No camera found in the scene!");
        }
    }
    
    // Метод для обновления ссылки на игрока
    private void UpdatePlayerReference()
    {
        // Находим все объекты DragAndDropItem и обновляем ссылку на игрока
        DragAndDropItem[] dragItems = FindObjectsOfType<DragAndDropItem>();
        foreach (DragAndDropItem dragItem in dragItems)
        {
            dragItem.UpdatePlayerReference();
        }
    }
    
    // Методы для открытия/закрытия инвентаря
    private void OpenInventory()
    {
        UIBG.SetActive(true);
        crosshair.SetActive(false);
        inventoryPanel.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private void CloseInventory()
    {
        UIBG.SetActive(false);
        crosshair.SetActive(true);
        inventoryPanel.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    // Публичный метод для принудительного обновления камеры из других скриптов
    public void RefreshCamera()
    {
        cameraNeedsUpdate = true;
    }
    public void UpdateCameraReference() 
    {
        mainCamera = Camera.main;
    }
    private void AddItem(ItemScriptableObject _item, int _amount)
    {
        // ... существующий код AddItem ...
        foreach(InventorySlot slot in hotbarSlots)
        {
            if(slot.item == _item && slot.amount + _amount <= _item.maximumAmount)
            {
                slot.amount += _amount;
                slot.itemAmountText.text = slot.amount.ToString();
                return;
            }
        }
        
        foreach(InventorySlot slot in hotbarSlots)
        {
            if (slot.isEmpty)
            {
                slot.item = _item;
                slot.amount = _amount;
                slot.isEmpty = !slot.isEmpty;
                slot.SetIcon(_item.icon);
                slot.itemAmountText.text = slot.amount > 1 ? slot.amount.ToString() : "";
                return;
            }
        }
        
        foreach(InventorySlot slot in slots)
        {
            if(slot.item == _item && slot.amount + _amount <= _item.maximumAmount)
            {
                slot.amount += _amount;
                slot.itemAmountText.text = slot.amount.ToString();
                return;
            }
        }
        
        foreach(InventorySlot slot in slots)
        {
            if (slot.isEmpty)
            {
                slot.item = _item;
                slot.amount = _amount;
                slot.isEmpty = !slot.isEmpty;
                slot.SetIcon(_item.icon);
                slot.itemAmountText.text = slot.amount > 1 ? slot.amount.ToString() : "";
                return;
            }
        }
    }
}