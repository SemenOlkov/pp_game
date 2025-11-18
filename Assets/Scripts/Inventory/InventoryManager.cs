using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public GameObject UIPanel;
    public Transform inventoryPanel;
    public List<InventorySlot> slots = new List<InventorySlot>();
    public bool isOpened;
    
    private Keyboard keyboard;
    private Mouse mouse;
    private Camera mainCamera;
    public float reachDistance = 10f;
    public GameObject crosshair;
    
    // Добавлены новые переменные
    public GameObject itemPickupCanvas; // Канвас для подсказки подбора предмета
    
    void Start()
    {
        mainCamera = Camera.main;
        UIPanel.SetActive(false);
        if (itemPickupCanvas != null)
            itemPickupCanvas.SetActive(false); // Деактивируем канвас при старте
        
        for(int i = 0; i < inventoryPanel.childCount; i++)
        {
            if(inventoryPanel.GetChild(i).GetComponent<InventorySlot>() != null)
            {
                slots.Add(inventoryPanel.GetChild(i).GetComponent<InventorySlot>());
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
                UIPanel.SetActive(false);
                crosshair.SetActive(true);

            }
            else
            {
                UIPanel.SetActive(true);
                crosshair.SetActive(false);
            }
            isOpened = !isOpened;
        }
        
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        bool isUIPanelActive = UIPanel.activeInHierarchy;

        // Если UIPanel не активна и луч попал в объект
        if(!isUIPanelActive && Physics.Raycast(ray, out hit, reachDistance))
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
            // Деактивируем канвас если UIPanel активна или не смотрим на предмет
            if (itemPickupCanvas != null)
                itemPickupCanvas.SetActive(false);
        }
    }
    
    private void AddItem(ItemScriptableObject _item, int _amount)
    {
        foreach(InventorySlot slot in slots)
        {
            if(slot.item == _item)
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