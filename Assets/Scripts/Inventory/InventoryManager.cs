using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public GameObject UIBG;
    public Transform inventoryPanel;
    public Transform hotbarPanel;
    public List<InventorySlot> slots = new List<InventorySlot>();
    public List<InventorySlot> hotbarSlots =  new List<InventorySlot>();
    public bool isOpened;
    // public CinemachineVirtualCamera CVC;
    
    private Keyboard keyboard;
    private Mouse mouse;
    private Camera mainCamera;
    public float reachDistance = 10f;
    public GameObject crosshair;
    
    // Добавлены новые переменные
    public GameObject itemPickupCanvas; // Канвас для подсказки подбора предмета
    
    void Start()
    {   
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mainCamera = Camera.main;
        UIBG.SetActive(false);
        inventoryPanel.gameObject.SetActive(false);
        if (itemPickupCanvas != null)
            itemPickupCanvas.SetActive(false); // Деактивируем канвас при старте
        
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
        if (keyboard != null && keyboard.eKey.wasPressedThisFrame)
        {
            if (isOpened)
            {
                UIBG.SetActive(false);
                crosshair.SetActive(true);
                inventoryPanel.gameObject.SetActive(false);
                // HotbarUIBG.SetActive(true);
                // CVC.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisName = "";
                // CVC.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisName = "";
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                UIBG.SetActive(true);
                crosshair.SetActive(false);
                inventoryPanel.gameObject.SetActive(true);
                // HotbarUIBG.SetActive(false);
                // CVC.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisName = "Mouse Y";
                // CVC.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisName = "Mouse X";
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            isOpened = !isOpened;
        }
        
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        bool isUIBGActive = UIBG.activeInHierarchy;

        // Если UIBG не активна и луч попал в объект
        if(!isUIBGActive && Physics.Raycast(ray, out hit, reachDistance))
        {
            if(hit.collider.gameObject.GetComponent<Item>() != null)
            {
                // Активируем канвас при наведении на предмет
                if (itemPickupCanvas != null)
                    itemPickupCanvas.SetActive(true);
                
                // Проверяем нажатие ЛКМ для подбора предмета (исправлено на Input System)
                if(mouse != null && mouse.leftButton.wasPressedThisFrame)
                {
                    AddItem(hit.collider.gameObject.GetComponent<Item>().item, hit.collider.gameObject.GetComponent<Item>().amount);
                    Destroy(hit.collider.gameObject);
                    // Деактивируем канвас после подбора
                    if (itemPickupCanvas != null)
                        itemPickupCanvas.SetActive(false);
                }
            }
            else
            {
                // Деактивируем канвас если смотрим не на предмет
                if (itemPickupCanvas != null)
                    itemPickupCanvas.SetActive(false);
            }
        }
        else
        {
            // Деактивируем канвас если UIBG активна или не смотрим на предмет
            if (itemPickupCanvas != null)
                itemPickupCanvas.SetActive(false);
        }
    }
    
    private void AddItem(ItemScriptableObject _item, int _amount)
    {
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