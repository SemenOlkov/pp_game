using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DoorKeyChecker : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private float reachDistance = 5f;

    [Header("UI Text Hints")]
    [SerializeField] private GameObject noKeyHint;       // "У вас нет ключа в руках"
    [SerializeField] private GameObject wrongTypeHint;   // "Этот ключ не подходит (неверная длина кода)"
    [SerializeField] private GameObject readyHint;
    [SerializeField] private string sceneName;        // "Нажмите Enter, чтобы вставить ключ"

    [Header("Logic Settings")]
    [SerializeField] private string correctKeyCode = "1";

    private bool isHoveringDoor = false;

    // Геттеры для динамического получения объектов
    private Camera MainCamera => Camera.main;
    
    private HotbarPanel CurrentHotbar 
    {
        get 
        {
            if (PersistentObject.Instance != null)
                return PersistentObject.Instance.GetComponentInChildren<HotbarPanel>(true);
            return null;
        }
    }

    void Update()
    {
        CheckDoorRaycast();

        if (isHoveringDoor)
        {
            HandleInteraction();
        }
    }

    private void CheckDoorRaycast()
    {
        Camera cam = MainCamera;
        if (cam == null) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        if (Physics.Raycast(ray, out RaycastHit hit, reachDistance, interactionLayer))
        {
            if (!isHoveringDoor)
            {
                isHoveredDoor(true);
            }
            UpdateUI();
        }
        else
        {
            if (isHoveringDoor)
            {
                isHoveredDoor(false);
            }
        }
    }

    private void isHoveredDoor(bool state)
    {
        isHoveringDoor = state;
        if (!state) HideAllHints();
    }

    private void UpdateUI()
    {
        HideAllHints();

        var selectedItem = GetSelectedItem();

        // 1. Проверка: вообще нет предмета или это не ключ, или длина кода не 1
        if (selectedItem == null || selectedItem.itemType != ItemType.Key)
        {
            if (noKeyHint != null) noKeyHint.SetActive(true);
            return;
        }

        KeyItem key = selectedItem as KeyItem;
        
        // 2. Проверка: это ключ, но длина кода не равна 1
        if (key == null || key.keyCode.Length != 1)
        {
            if (wrongTypeHint != null) wrongTypeHint.SetActive(true);
        }
        // 3. Условия соблюдены (Ключ и длина кода == 1)
        else
        {
            if (readyHint != null) readyHint.SetActive(true);
        }
    }

    private void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            var selectedItem = GetSelectedItem();
            
            // Проверяем условия еще раз перед активацией
            if (selectedItem != null && selectedItem.itemType == ItemType.Key)
            {
                KeyItem key = selectedItem as KeyItem;
                if (key != null && key.keyCode.Length == 1)
                {
                    if (key.keyCode == correctKeyCode)
                    {
                        SceneManager.LoadScene(sceneName);
                    }
                    else
                    {
                        ExecuteDeath();
                    }
                }
            }
        }
    }

    private ItemScriptableObject GetSelectedItem()
    {
        HotbarPanel hb = CurrentHotbar;
        if (hb == null) return null;

        // Берем предмет из активного индекса хотбара
        if (hb.selectedSlotIndex >= 0 && hb.selectedSlotIndex < hb.slots.Count)
        {
            return hb.slots[hb.selectedSlotIndex].item;
        }
        return null;
    }

    private void HideAllHints()
    {
        if (noKeyHint != null) noKeyHint.SetActive(false);
        if (wrongTypeHint != null) wrongTypeHint.SetActive(false);
        if (readyHint != null) readyHint.SetActive(false);
    }

    private void ExecuteDeath()
    {
        Debug.Log("Неверный ключ! Активация ловушки...");
        if (PersistentObject.Instance != null)
            Destroy(PersistentObject.Instance.gameObject);
            
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;    
        SceneManager.LoadScene("Dead");
    }
}