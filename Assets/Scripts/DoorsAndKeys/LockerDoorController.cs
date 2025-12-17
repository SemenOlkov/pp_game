using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockerDoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Transform doorTransform; 
    [SerializeField] private float openAngle = -90f; 
    [SerializeField] private float openSpeed = 90f; 
    [SerializeField] private bool isOpen = false; 

    [Header("Key Settings")]
    [SerializeField] private bool requiresKey = false; 
    [SerializeField] private string requiredKeyCode = "333"; 

    [Header("Trap settings")]
    [SerializeField] private bool isTrap = false;
    [SerializeField] private GameObject screamerTrigger;

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask lockerLayer; 
    [SerializeField] private float interactionDistance = 8f; 

    [Header("UI Settings")]
    [SerializeField] private GameObject openHintText; 
    [SerializeField] private GameObject closeHintText; 
    [SerializeField] private GameObject lockedHintText; 

    private bool isAnimating = false;
    private Coroutine doorCoroutine;
    private bool isHovered = false;
    private Quaternion closedRotation; 
    private Quaternion openRotation;

    // --- ДИНАМИЧЕСКИЕ ССЫЛКИ ---

    private Camera PlayerCamera => Camera.main;

    private InventoryManager CurrentInventory => InventoryManager.Instance;

    // ---------------------------

    void Start()
    {
        HideAllHints();
        
        if (doorTransform == null)
        {
            doorTransform = transform.Find("Door");
            if (doorTransform == null)
                Debug.LogError($"Door не найден в {gameObject.name}!");
        }
        
        if (doorTransform != null)
        {
            closedRotation = doorTransform.localRotation;
            openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
            
            if (isOpen) doorTransform.localRotation = openRotation;
        }

        if (screamerTrigger != null) screamerTrigger.SetActive(false);
    }

    void Update()
    {
        HandleRaycast();
        HandleInput();
    }

    private void HandleRaycast()
    {
        Camera cam = PlayerCamera;
        if (cam == null) return;

        // Луч из центра экрана
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, lockerLayer))
        {
            // Проверяем, что попали именно в этот объект или его дочерние элементы
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                if (!isHovered)
                {
                    isHovered = true;
                    ShowHint();
                }
                return;
            }
        }
        
        if (isHovered)
        {
            isHovered = false;
            HideAllHints();
        }
    }

    private void HandleInput()
    {
        // Проверяем нажатие правой кнопки мыши (ПКМ)
        if (Input.GetMouseButtonDown(1) && isHovered)
        {
            ToggleDoor();
        }
    }

    private void ShowHint()
    {
        HideAllHints();
        
        if (isOpen)
        {
            if (closeHintText != null) closeHintText.SetActive(true);
        }
        else
        {
            if (requiresKey && !HasRequiredKey())
            {
                if (lockedHintText != null) lockedHintText.SetActive(true);
            }
            else
            {
                if (openHintText != null) openHintText.SetActive(true);
            }
        }
    }

    private void HideAllHints()
    {
        if (openHintText != null) openHintText.SetActive(false);
        if (closeHintText != null) closeHintText.SetActive(false);
        if (lockedHintText != null) lockedHintText.SetActive(false);
    }

    private void ToggleDoor()
    {
        if (isAnimating || doorTransform == null) return;

        if (!isOpen && requiresKey && !HasRequiredKey())
        {
            Debug.Log($"Дверь заперта! Нужен ключ: {requiredKeyCode}");
            // Здесь можно добавить звук "дёрганья" запертой ручки
            return;
        }

        if (doorCoroutine != null) StopCoroutine(doorCoroutine);
        doorCoroutine = StartCoroutine(AnimateDoor(!isOpen));
    }

    private IEnumerator AnimateDoor(bool open)
    {
        isAnimating = true;
        
        Quaternion startRotation = doorTransform.localRotation;
        Quaternion targetRotation = open ? openRotation : closedRotation;
        
        float angle = Quaternion.Angle(startRotation, targetRotation);
        float duration = angle / Mathf.Max(openSpeed, 1f); 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            doorTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            yield return null;
        }

        doorTransform.localRotation = targetRotation;
        isOpen = open;
        isAnimating = false;

        // Если это ловушка и мы открыли дверь
        if (isOpen && isTrap && screamerTrigger != null)
        {
            screamerTrigger.SetActive(true);
        }
        
        if (isHovered) ShowHint();
    }

    private bool HasRequiredKey()
    {
        InventoryManager inv = CurrentInventory;
        if (inv == null) return false;

        // Создаем общий список слотов для проверки
        List<InventorySlot> allSlots = new List<InventorySlot>(inv.slots);
        allSlots.AddRange(inv.hotbarSlots);

        foreach (InventorySlot slot in allSlots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Key)
            {
                // Приведение типа к KeyItem (убедитесь, что ваш ScriptableObject называется так)
                KeyItem keyComponent = slot.item as KeyItem;
                if (keyComponent != null && keyComponent.keyCode == requiredKeyCode)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void UnlockAndOpen()
    {
        requiresKey = false;
        if (!isOpen) ToggleDoor();
    }

    public void OpenDoor() { if (!isOpen && !isAnimating) ToggleDoor(); }
    public void CloseDoor() { if (isOpen && !isAnimating) ToggleDoor(); }

    void OnDrawGizmosSelected()
    {
        if (doorTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(doorTransform.position, doorTransform.up);
        }
    }
}