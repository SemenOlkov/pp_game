using System.Collections.Generic;
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
    
    void Start()
    {
        mainCamera = Camera.main;
        UIPanel.SetActive(false);
        
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
            }
            else
            {
                UIPanel.SetActive(true);
            }
            isOpened = !isOpened;
        }
        
        // Используем новую Input System вместо старой
        if (mouse != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(mouse.position.ReadValue());
            RaycastHit hit;
            Debug.Log("1");
            if(Physics.Raycast(ray, out hit, reachDistance))
            {
                Debug.Log(2);
                Debug.DrawRay(ray.origin, ray.direction * reachDistance, Color.green);
                if(hit.collider.gameObject.GetComponent<Item>() != null)
                {
                    Debug.Log(3);
                    AddItem(hit.collider.gameObject.GetComponent<Item>().item, hit.collider.gameObject.GetComponent<Item>().amount);
                    Destroy(hit.collider.gameObject.GetComponent<Item>());
                }
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * reachDistance, Color.red);
            }
        }
    }
    
    private void AddItem(ItemScriptableObject _item, int _amount)
    {
        foreach(InventorySlot slot in slots)
        {
            if(slot.item == _item)
            {
                slot.amount += _amount;
                return;
            }
        }
        
        foreach(InventorySlot slot in slots)
        {
            if (slot.isEmpty)
            {
                slot.item = _item;
                slot.amount = _amount;
                return;
            }
        }
    }
}