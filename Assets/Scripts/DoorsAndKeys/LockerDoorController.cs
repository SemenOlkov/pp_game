using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockerDoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Transform doorTransform; // Ссылка на трансформ двери
    [SerializeField] private float openAngle = -90f; // Угол открытия
    [SerializeField] private float openSpeed = 90f; // Скорость открытия
    [SerializeField] private bool isOpen = false; // Текущее состояние

    [Header("Key Settings")]
    [SerializeField] private bool requiresKey = false; // Требуется ли ключ для открытия
    [SerializeField] private string requiredKeyCode = "333"; // Код требуемого ключа

    [Header("Raycast Settings")]
    [SerializeField] private LayerMask lockerLayer; // Слой шкафчиков
    [SerializeField] private float interactionDistance = 3f; // Дистанция взаимодействия

    [Header("UI Settings")]
    [SerializeField] private GameObject openHintText; // Текст подсказки для открытия
    [SerializeField] private GameObject closeHintText; // Текст подсказки для закрытия
    [SerializeField] private GameObject lockedHintText; // Текст подсказки, что заперто (нет ключа)

    [Header("Inventory Reference")]
    [SerializeField] private InventoryManager inventoryManager; // Ссылка на InventoryManager

    private Camera playerCamera;
    private bool isAnimating = false;
    private Coroutine doorCoroutine;
    private bool isHovered = false;
    private Quaternion closedRotation; // Начальное (закрытое) вращение
    private Quaternion openRotation; // Открытое вращение

    void Start()
    {
        // Находим главную камеру
        playerCamera = Camera.main;
        
        // Скрываем все подсказки на старте
        HideAllHints();
        
        // Автоматически находим дверь если не задана
        if (doorTransform == null)
        {
            doorTransform = transform.Find("Door");
            if (doorTransform == null)
                Debug.LogError("Door not found! Please assign door transform.");
        }
        
        // Сохраняем начальное вращение как закрытое состояние
        closedRotation = doorTransform.localRotation;
        
        // Вычисляем открытое вращение на основе начального
        // Поворачиваем от начального вращения на openAngle градусов вокруг локальной оси Y
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
        
        // Если дверь изначально должна быть открыта, устанавливаем открытое вращение
        if (isOpen)
        {
            doorTransform.localRotation = openRotation;
        }
        
        // Если inventoryManager не присвоен, пытаемся найти его
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }
    }

    void Update()
    {
        HandleRaycast();
        HandleInput();
    }

    private void HandleRaycast()
    {
        if (playerCamera == null) return;

        // Создаем луч из центра экрана
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Проверяем пересечение луча с объектами на слое шкафчика
        if (Physics.Raycast(ray, out hit, interactionDistance, lockerLayer))
        {
            // Проверяем, попал ли луч именно в этот шкафчик
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
        
        // Если луч не попал в шкафчик - скрываем подсказку
        if (isHovered)
        {
            isHovered = false;
            HideAllHints();
        }
    }

    private void HandleInput()
    {
        // Проверяем нажатие ПКМ при наведении
        if (Input.GetMouseButtonDown(1) && isHovered)
        {
            ToggleDoor();
        }
    }

    private void ShowHint()
    {
        // Сначала скрываем все подсказки
        HideAllHints();
        
        // Показываем нужную подсказку
        if (isOpen)
        {
            // Дверь открыта - показываем подсказку закрытия
            if (closeHintText != null)
                closeHintText.SetActive(true);
        }
        else
        {
            // Дверь закрыта - проверяем, нужен ли ключ и есть ли он
            if (requiresKey && !HasRequiredKey())
            {
                // Нет нужного ключа - показываем подсказку "заперто"
                if (lockedHintText != null)
                    lockedHintText.SetActive(true);
            }
            else
            {
                // Ключ есть или не требуется - показываем подсказку открытия
                if (openHintText != null)
                    openHintText.SetActive(true);
            }
        }
    }

    private void HideAllHints()
    {
        if (openHintText != null)
            openHintText.SetActive(false);
        if (closeHintText != null)
            closeHintText.SetActive(false);
        if (lockedHintText != null)
            lockedHintText.SetActive(false);
    }

    private void ToggleDoor()
    {
        if (isAnimating || doorTransform == null) return;

        // Если пытаемся открыть закрытую дверь, проверяем ключ
        if (!isOpen && requiresKey && !HasRequiredKey())
        {
            // Воспроизвести звук закрытой двери или показать сообщение
            Debug.Log("Дверь заперта! Нужен ключ с кодом: " + requiredKeyCode);
            return;
        }

        // Останавливаем предыдущую анимацию если она есть
        if (doorCoroutine != null)
            StopCoroutine(doorCoroutine);

        // Запускаем новую анимацию
        doorCoroutine = StartCoroutine(AnimateDoor(!isOpen));
    }

    private IEnumerator AnimateDoor(bool open)
    {
        isAnimating = true;
        
        Quaternion startRotation = doorTransform.localRotation;
        Quaternion targetRotation = open ? openRotation : closedRotation;
        
        float time = 0f;
        float duration = Quaternion.Angle(startRotation, targetRotation) / openSpeed;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            doorTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        doorTransform.localRotation = targetRotation;
        isOpen = open;
        isAnimating = false;
        
        // Обновляем подсказку после анимации
        if (isHovered)
            ShowHint();
    }

    // Проверяем, есть ли у игрока нужный ключ
    private bool HasRequiredKey()
    {
        if (inventoryManager == null)
        {
            Debug.LogWarning("InventoryManager не назначен в LockerDoorController!");
            return false;
        }

        // Проверяем основные слоты инвентаря
        foreach (InventorySlot slot in inventoryManager.slots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Key)
            {
                // Проверяем, есть ли у предмета компонент Key с нужным кодом
                KeyItem keyComponent = slot.item as KeyItem;
                if (keyComponent != null && keyComponent.keyCode == requiredKeyCode)
                {
                    return true;
                }
            }
        }
        
        // Проверяем слоты горячей панели
        foreach (InventorySlot slot in inventoryManager.hotbarSlots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Key)
            {
                KeyItem keyComponent = slot.item as KeyItem;
                if (keyComponent != null && keyComponent.keyCode == requiredKeyCode)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Метод для принудительного открытия (например, из другого скрипта)
    public void UnlockAndOpen()
    {
        requiresKey = false;
        if (!isOpen && !isAnimating)
        {
            ToggleDoor();
        }
    }

    // Методы для открытия/закрытия из других скриптов
    public void OpenDoor()
    {
        if (!isOpen && !isAnimating)
        {
            if (doorCoroutine != null)
                StopCoroutine(doorCoroutine);
            doorCoroutine = StartCoroutine(AnimateDoor(true));
        }
    }

    public void CloseDoor()
    {
        if (isOpen && !isAnimating)
        {
            if (doorCoroutine != null)
                StopCoroutine(doorCoroutine);
            doorCoroutine = StartCoroutine(AnimateDoor(false));
        }
    }

    // Вспомогательный метод для проверки, наведен ли курсор на этот шкафчик
    public bool IsHovered()
    {
        return isHovered;
    }

    // Публичный метод для проверки, заперта ли дверь
    public bool IsLocked()
    {
        return requiresKey && !HasRequiredKey();
    }

    // Для отладки: визуализация начального и конечного вращения
    void OnDrawGizmosSelected()
    {
        if (doorTransform != null && Application.isPlaying)
        {
            // Показываем закрытое положение
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.1f, 0.1f, 0.1f));
            
            // Показываем ось вращения (примерно)
            Vector3 pivotPoint = doorTransform.localPosition;
            Gizmos.DrawLine(pivotPoint, pivotPoint + new Vector3(0, 0, 0.5f));
        }
    }
}